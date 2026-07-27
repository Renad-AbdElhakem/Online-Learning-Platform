
using CategoryService.Data;
using CategoryService.EndPoints;
using CategoryService.Services;
using Consul;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace CategoryService
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

            builder.Services.AddDbContext<CategoryDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("CategoryServiceDB")));

            //1- Add localhost (location) to regist at  
            builder.Services.AddSingleton<IConsulClient, ConsulClient>(p =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");
                }));

            builder.Services.AddHealthChecks();

            builder.Services.AddScoped<ICategoryService, CategoryServices>();

            var app = builder.Build();

            // 2- Register the service

            var consulClient = app.Services.GetRequiredService<IConsulClient>();

            var registration = new AgentServiceRegistration
            {
                ID = "CategoryService-1",
                Name = "categoriesService",
                Address = "localhost",
                Port = 5136,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5136/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }

            };

            consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();


            //3- Deregister on Shutdown

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(() =>
            {

                consulClient.Agent.ServiceDeregister("CategoryServiice-1").GetAwaiter().GetResult();

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
            app.MapCategoryEndPoints();


            app.Run();
        }
    }
}
