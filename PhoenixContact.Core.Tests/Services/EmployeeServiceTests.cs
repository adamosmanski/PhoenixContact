using Moq;
using Moq.Protected;
using PhoenixContact.Core.Model;
using PhoenixContact.Core.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PhoenixContact.Core.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly LoggingService _loggingService;
        public EmployeeServiceTests()
        {
            _loggingService = new LoggingService(new HttpClient());
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnList_WhenApiReturnsSuccess()
        {
            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = 1, FirstName = "Anna", LastName = "Nowak", Salary = 5000,
                                PositionLevel = "S3", Residence = "Warszawa" }
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(employees)
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var service = new EmployeeService(httpClient, _loggingService);

            var result = await service.GetAllEmployeesAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Anna", result[0].FirstName);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnEmptyList_WhenApiFails()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new EmployeeService(httpClient, _loggingService);

            var result = await service.GetAllEmployeesAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SendEmployeesToApiAsync_ShouldReturnFalse_WhenEmployeesIsNull()
        {
            var httpClient = new HttpClient();
            var service = new EmployeeService(httpClient, _loggingService);

            var result = await service.SendEmployeesToApiAsync(null);

            Assert.False(result);
        }

        [Fact]
        public async Task ParseCsvFileAsync_ShouldReturnEmployeesList_WhenValidCsv()
        {
            string csv = "Id;FirstName;LastName;Salary;PositionLevel;Residence\n1;Kuba;Kowalski;4500;S1;Kraków";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
            var service = new EmployeeService(new HttpClient(), _loggingService);

            var result = await service.ParseCsvFileAsync(stream);

            Assert.Single(result);
            Assert.Equal("Kuba", result[0].FirstName);
            Assert.Equal("Kraków", result[0].Residence);
        }
    }
}