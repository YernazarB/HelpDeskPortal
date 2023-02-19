using HelpDesk.Infrastructure.Options;
using HelpDesk.Infrastructure;
using HelpDesk.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using MassTransit;

namespace HelpDeskPortal.Extensions
{
    public static class StartupExtensions
    {
        public static void AddSwaggerWithAuth(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen(setup =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }

        public static void AddAuthenticationAndAuthorization(this IServiceCollection services, AuthOptions authOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Secret)),
                    ValidateIssuerSigningKey = true
                };
            });
            services.AddAuthorization();
        }

        public static async Task MigrateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();

                var section = app.Configuration.GetSection("GlobalAdmin");
                var options = section.Get<GlobalAdminOptions>();
                await db.AddGlobalAdminIfNotExists(options);
            }
        }

        public static void AddRedisCache(this IServiceCollection services, IConfiguration configs)
        {
            var redisConfigs = configs.GetSection("Redis");

            services.AddStackExchangeRedisCache(x =>
                x.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints = new EndPointCollection 
                    { 
                        $"{redisConfigs["Endpoint"]}:{redisConfigs["Port"]}" 
                    },
                    User = redisConfigs["Username"],
                    Password = redisConfigs["Password"]
                });
        }

        public static async Task ClearCache(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

            await cache.RemoveAsync(CacheHelper.OrganizationsKey);
        }

        public static void AddMessageBroker(this IServiceCollection services, MessageBrokerOptions options)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.SetKebabCaseEndpointNameFormatter();
                busConfig.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(options.Host), h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });
                });
            });
        }
    }
}
