
using Consul;
using Yarp.ReverseProxy.ServiceDiscovery;

namespace GatewayProject
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


            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");
                })

                );

            builder.Services.AddSingleton<IDestinationResolver, ConsulDestinationResolver>();


            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection(""));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapReverseProxy();


            app.Run();
        }
    }
}
