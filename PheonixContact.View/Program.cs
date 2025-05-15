using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PheonixContact.View;
using PhoenixContact.Core.Services;

namespace PhoenixContact.View
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddScoped<LoggingService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7124/")
            });
            await builder.Build().RunAsync();
        }
    }
}
