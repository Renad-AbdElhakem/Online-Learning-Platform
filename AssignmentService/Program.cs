
using AssignmentService.Api;
using AssignmentService.Data;
using AssignmentService.ExternalService;
using AssignmentService.Model;
using AssignmentService.Service;
using Consul;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace AssignmentService
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


            builder.Services.AddDbContext<AssignmentDbContext>(option =>
           option.UseSqlServer(builder.Configuration.GetConnectionString("AssignmentServiceDB")));


            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
            new ConsulClient(config =>
            {
                config.Address = new Uri("http://localhost:8500");
            }));

            builder.Services.AddHealthChecks();
            builder.Services.AddHttpClient<GroupClient>();
            builder.Services.AddHttpClient<StudentClient>();

            builder.Services.AddScoped<IAssignmentService, AssignmentServices>();



            var app = builder.Build();


            var consulClient = app.Services.GetRequiredService<IConsulClient>();


            var assignmentServiceRegistration = new AgentServiceRegistration
            {
                ID = "Assignment-service-1",
                Name = "AssignmentService",
                Address = "localhost",
                Port = 5082,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5082/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            await consulClient.Agent.ServiceRegister(assignmentServiceRegistration);

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>

                await consulClient.Agent.ServiceDeregister("Assignment-service-1")

            );


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseHealthChecks("/health");
           app.MapAssignmentEndPoints();
            app.MapStudentAssignmentEndPoints();
            app.Run();
        }
    }
}
