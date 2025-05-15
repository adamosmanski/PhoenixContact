using PhoenixContact.Core.Model;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;

namespace PhoenixContact.Core.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> ParseCsvFileAsync(Stream csvStream);
        Task<bool> SendEmployeesToApiAsync(List<EmployeeDto> employees);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<List<EmployeeDto>> GetTopEarnersByCityAsync();
        Task<List<EmployeeDto>> GetTopEarnersByPositionAsync();
        Task<List<EmployeeDto>> GetLowestEarnersByCityAsync();
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly LoggingService _loggingService;
        public EmployeeService(HttpClient httpClient, LoggingService loggingService)
        {
            _httpClient = httpClient;
            _loggingService = loggingService;
        }
        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/employees");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                }
                else
                {
                    _loggingService.SendErrorLogAsync($"Błąd podczas pobierania danych: {response.StatusCode}");
                    return new List<EmployeeDto>();
                }
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogAsync(ex.Message, ex.StackTrace);
                return new List<EmployeeDto>();
            }
        }

        public async Task<List<EmployeeDto>> ParseCsvFileAsync(Stream csvStream)
        {
            var employees = new List<EmployeeDto>();

            using var reader = new StreamReader(csvStream);
            var content = await reader.ReadToEndAsync();
            var lines = content.Split('\n').Skip(1);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(';');
                if (values.Length < 6) continue;

                try
                {
                    employees.Add(new EmployeeDto
                    {
                        Id = int.Parse(values[0].Trim()),
                        FirstName = values[1].Trim(),
                        LastName = values[2].Trim(),
                        Salary = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                        PositionLevel = values[4].Trim(),
                        Residence = values[5].Trim()
                    });
                }
                catch
                {
                    continue;
                }
            }

            return employees;
        }

        public async Task<bool> SendEmployeesToApiAsync(List<EmployeeDto> employees)
        {
            if (employees == null || !employees.Any())
                return false;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/employees/import", employees);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogAsync(ex.Message, ex.StackTrace);
                return false;
            }
        }
        public async Task<List<EmployeeDto>> GetTopEarnersByCityAsync()
        {
            try
            {
                var allEmployees = await GetAllEmployeesAsync();
                return allEmployees
                    .GroupBy(e => e.Residence)
                    .Select(g => g.OrderByDescending(e => e.Salary).First())
                    .OrderBy(e => e.Residence)
                    .ToList();
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogAsync(ex.Message, ex.StackTrace);
                return new List<EmployeeDto>();
            }
        }
        public async Task<List<EmployeeDto>> GetTopEarnersByPositionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/employees/top-earners-by-position");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                }
                else
                {
                    _loggingService.SendErrorLogAsync(response.StatusCode.ToString());
                    return new List<EmployeeDto>();
                }
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogAsync(ex.Message, ex.StackTrace);
                return new List<EmployeeDto>();
            }
        }
        public async Task<List<EmployeeDto>> GetLowestEarnersByCityAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/employees/lowest-earners-by-city");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                }
                else
                {
                    _loggingService.SendErrorLogAsync(response.StatusCode.ToString());
                    return new List<EmployeeDto>();
                }
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogAsync(ex.Message, ex.StackTrace);
                return new List<EmployeeDto>();
            }
        }

    }
}