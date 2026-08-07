
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Yarp.ReverseProxy.ServiceDiscovery;

namespace GatewayProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<IDestinationResolver, ConsulDestinationResolver>();

            builder.Services.AddAuthorization();


            builder.Services.AddSingleton<IConsulClient, ConsulClient>(opt =>
                new ConsulClient(config =>
                {
                    config.Address = new Uri("http://localhost:8500");
                })

                );

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

          


            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Category-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 150,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 50,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });



            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Course-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 120,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 40,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("InstructorCourse-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 120,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 40,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Instructor-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 120,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 40,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });


            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Group-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 30,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Student-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 20,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });




            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Enrollment-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 80,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 20,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });



            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Lecture-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 60,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 20,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });


            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Material-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 50,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 10,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });


            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Assignment-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 30,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 5,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("StudentAssignment-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 3,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("UserAuth-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    ReplenishmentPeriod = TimeSpan.FromHours(5),
                    TokensPerPeriod = 2,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddPolicy("Role-per-user", httpcontext =>
                RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpcontext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "annonymous",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    TokensPerPeriod = 40,
                    QueueLimit = 0
                }));
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });



            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapReverseProxy();


            app.Run();
        }
    }
}
