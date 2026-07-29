
using Consul;
using GroupsCourseService.Api;
using GroupsCourseService.Data;
using GroupsCourseService.ExternalService;
using GroupsCourseService.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Threading.Tasks;

namespace GroupsCourseService
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

            builder.Services.AddDbContext<GroupsCourseDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("GroupsServiceDB")));

            builder.Services.AddScoped<IGroupService, GroupService>();

            builder.Services.AddHttpClient<CourseClient>();
            builder.Services.AddHttpClient<InstructorClient>();

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
            new ConsulClient(config =>
            {
                config.Address = new Uri("http://localhost:8500");
            }));

            builder.Services.AddHealthChecks();



            var app = builder.Build();


            var consulClient = app.Services.GetRequiredService<IConsulClient>();


            var groupServiceRegistration = new AgentServiceRegistration
            {
                ID = "Groups-service-1",
                Name = "GroupsService",
                Address = "localhost",
                Port = 5111,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5111/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };

            await consulClient.Agent.ServiceRegister(groupServiceRegistration);

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>

                await consulClient.Agent.ServiceDeregister("Groups-service-1")

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
            app.RegisterRoutes();




            app.Run();
        }
    }
}
