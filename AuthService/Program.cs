
using AuthService.Api;
using AuthService.Data;
using AuthService.Service;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace AuthService
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

            builder.Services.AddDbContext<AuthDbContext>(option =>
          option.UseSqlServer(builder.Configuration.GetConnectionString("AuthServiceDB")));

            builder.Services.AddScoped<IUserAuthService, UserAuthService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddHealthChecks();

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");
                }));




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
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:SecurityKey"]!))
                   };
               });

            var app = builder.Build();

            var consulClient = app.Services.GetRequiredService<IConsulClient>();


            var AuthServiceRegistration = new AgentServiceRegistration
            {
                ID = "Auth-service-1",
                Name = "AuthService",
                Address = "localhost",
                Port = 5271,
                Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:5271/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)

                }
            };
           

            await consulClient.Agent.ServiceRegister(AuthServiceRegistration);

        

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(async () =>

                await consulClient.Agent.ServiceDeregister("Auth-service-1")

            );
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

        //    app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");

            app.MapUserAuthEndPoints();
            app.MapRoleEndpoints();

            app.Run();
        }
    }
}
