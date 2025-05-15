using PhoenixContact.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixContact.Core.Services
{
    public class LoggingService
    {
        private readonly HttpClient _httpClient;

        public LoggingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendErrorLogAsync(string message, string stackTrace = null, string additionalInfo = null)
        {
            var log = new ErrorLogDto
            {
                Message = message,
                StackTrace = stackTrace,
                AdditionalInfo = additionalInfo
            };

            try
            {
                await _httpClient.PostAsJsonAsync("api/logs/error", log);
            }
            catch
            {
            
            }
        }
    }
}

