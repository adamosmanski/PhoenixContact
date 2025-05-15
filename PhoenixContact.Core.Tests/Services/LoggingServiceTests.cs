using Moq;
using Moq.Protected;
using PhoenixContact.Core.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PhoenixContact.Core.Tests.Services
{
    public class LoggingServiceTests
    {
        [Fact]
        public async Task SendErrorLogAsync_ShouldPostErrorLog()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/logs/error")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var service = new LoggingService(httpClient);

            await service.SendErrorLogAsync("Test error", "stack trace", "additional info");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/logs/error")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendErrorLogAsync_ShouldComplete_WhenHttpRequestFails()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var service = new LoggingService(httpClient);

            var exception = await Record.ExceptionAsync(() =>
                service.SendErrorLogAsync("Error", "Trace", "Info"));

            Assert.Null(exception);
        }

        [Fact]
        public async Task SendErrorLogAsync_ShouldReturn_WhenServerReturnsError()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var service = new LoggingService(httpClient);

            var exception = await Record.ExceptionAsync(() =>
                service.SendErrorLogAsync("Error", "Trace", "Info"));

            Assert.Null(exception);
        }
    }
}