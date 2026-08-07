
using CategoryService.Data;
using CategoryService.EndPoints;
using CategoryService.Services;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.Tasks;

namespace CategoryService
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

            builder.Services.AddDbContext<CategoryDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("CategoryServiceDB")));



            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["jwt:Issuer"],
                    ValidAudience = builder.Configuration["jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["jwt:SecurityKey"]!))
                };
            });





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

          await  consulClient.Agent.ServiceRegister(registration);


            //3- Deregister on Shutdown

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>
            {

               await consulClient.Agent.ServiceDeregister("CategoryServiice-1");

            });

            app.MapHealthChecks("/health");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapCategoryEndPoints();


            app.Run();
        }
    }
}
