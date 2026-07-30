
using Consul;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentService.Api;
using StudentService.Data;
using StudentService.Services;
using System.Threading.Tasks;

namespace StudentService
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

            builder.Services.AddDbContext<StudentDbContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("StudentConnection")));

            builder.Services.AddScoped<IStudentServices, StudentServices>();

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
            new ConsulClient(config =>
            {
                config.Address = new Uri("http://localhost:8500");
            }) );

            builder.Services.AddHealthChecks();


            var app = builder.Build();
         
            var consulclient = app.Services.GetRequiredService<IConsulClient>();

            var studentServiceRegisteration = new AgentServiceRegistration
            {
                ID = "Student-Service-1",
                Name = "StudentService",
                Address = "localhost",
                Port = 5094,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5094/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            await consulclient.Agent.ServiceRegister(studentServiceRegisteration);

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>
            {

                await consulclient.Agent.ServiceDeregister("Student-Service-1");

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
            app.MapStudentEndPoints();
            app.Run();
        }
    }
}
