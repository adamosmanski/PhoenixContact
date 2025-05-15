
using Microsoft.EntityFrameworkCore;
using PhoenixContact.Core.Services;
using PhoenixContact.EF;
using Serilog;

namespace PhoenixContact.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<PhoenixContactDb>(options => options.UseSqlServer(connectionString));
            builder.Services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001;http://localhost:5000");
            });

            var corsPolicyName = "AllowBlazorClient";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicyName, policy =>
                {
                    policy.WithOrigins("https://localhost:7175", "https://localhost:5001", "http://localhost:5000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            builder.Host.UseSerilog();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(corsPolicyName);
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
