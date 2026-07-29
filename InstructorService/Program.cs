
using Consul;
using InstructorService.Api;
using InstructorService.Data;
using InstructorService.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Threading.Tasks;

namespace InstructorService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<InstructorDbContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("InstructorConnection")) );

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");

                }));

            builder.Services.AddHealthChecks();

            builder.Services.AddScoped<IInstructorService, InstructorServices>();


            var app = builder.Build();


            var consulclient = app.Services.GetRequiredService<IConsulClient>();

            var instructorRegidteration = new AgentServiceRegistration
            {
                ID = "Instructor-Service-1",
                Name = "InstructorService",
                Address = "localhost",
                Port = 5093,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5093/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            await consulclient.Agent.ServiceRegister(instructorRegidteration);

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>
            {

                await consulclient.Agent.ServiceDeregister("Instructor-Service-1");

            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseHealthChecks("/health");
            app.MapInstructorEndpoint();

            app.Run();
        }
    }
}
