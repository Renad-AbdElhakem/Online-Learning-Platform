
using Consul;
using Content_Service.Api;
using Content_Service.ExternalService;
using Content_Service.Service;
using LectureService.Data;
using LectureService.Service;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace LectureService
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
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ContentDbContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("ContentServiceDB")));

            builder.Services.AddScoped<ILectureService, LectureServices>();

            builder.Services.AddHttpClient<GroupClient>();

            builder.Services.AddHealthChecks();

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");
                }));

            builder.Services.AddAntiforgery();


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            var app = builder.Build();

            var consulClient = app.Services.GetRequiredService<IConsulClient>();


            var contentServiceRegistration = new AgentServiceRegistration
            {
                ID = "Content-service-1",
                Name = "ContentService",
                Address = "localhost",
                Port = 5117,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5117/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            await consulClient.Agent.ServiceRegister(contentServiceRegistration);

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>

                await consulClient.Agent.ServiceDeregister("Content-service-1")

            );


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseHealthChecks("/health");
            app.MapLectureEndPoint();
            app.Run();
        }
    }
}
