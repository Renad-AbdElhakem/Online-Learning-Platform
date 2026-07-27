
using Consul;
using CourseService.CourseApi;
using CourseService.Data;
using CourseService.ExternalService;
using CourseService.Model;
using CourseService.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace CourseService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<CourseDBContext>(option =>
              option.UseSqlServer(builder.Configuration.GetConnectionString("CourseServiceDB")));

            builder.Services.AddHealthChecks();

            builder.Services.AddHttpClient<CategoryServiceClient>();

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
            new ConsulClient(config =>
            {
                config.Address = new Uri("http://localhost:8500");
            }));

            builder.Services.AddScoped<ICourseServices, CourseServices>();


            var app = builder.Build();


         
            var consulClient = app.Services.GetRequiredService<IConsulClient>();
            var registration = new AgentServiceRegistration
            {
                ID = "Course-service-1",
                Name = "CoursesService",
                Address = "localhost",
                Port = 5237,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5237/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();


            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(() =>
            {

                consulClient.Agent.ServiceDeregister("Course-service-1").GetAwaiter().GetResult();

            });

            app.MapHealthChecks("/health");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.MapCourseEndPoint();
           
            app.Run();
        }
    }
}
