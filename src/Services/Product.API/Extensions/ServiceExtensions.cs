using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;
using System.Text;
using Infrastructure.Identity;
using Microsoft.OpenApi.Models;

namespace Product.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.ConfigureSwagger();
            services.ConfigureProductDbContext(configuration);
            services.AddInfrastructrueService();
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
            //services.AddJwtAuthentication();
            services.ConfigureAuthenticationHandler();
            services.ConfigureAuthorization();
            services.ConfigureHealthChecks();
        }

        internal static void AddJwtAuthentication(this IServiceCollection services)
        {
            var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
            if (settings == null || string.IsNullOrEmpty(settings.Key))
                throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = false
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });
        }

        private static void ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
            if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
                throw new ArgumentNullException("Connection string is not configured.");

            var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);

            services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
                ServerVersion.AutoDetect(builder.ConnectionString), e =>
                {
                    e.MigrationsAssembly("Product.API");
                    e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                }));
        }

        private static void AddInfrastructrueService(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<IProductRepository, ProductRepository>();
        }

        private static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
            services
                .AddHealthChecks().AddMySql(databaseSettings.ConnectionString, null, null, "MySql Health",
                    HealthStatus.Degraded);
        }

        private static void ConfigureSwagger(this IServiceCollection services)
        {
            var configuration = services.GetOptions<ApiConfiguration>("ApiConfiguration");
            if (configuration == null || string.IsNullOrEmpty(configuration.IssuerUri) ||
                string.IsNullOrEmpty(configuration.ApiName)) throw new Exception("ApiConfiguration is not configured!");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Product API V1",
                        Version = configuration.ApiVersion
                    });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{configuration.IdentityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "tedu_microservices_api.read", "Tedu Microservices API Read Scope" },
                                { "tedu_microservices_api.write", "Tedu Microservices API Write Scope" }
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>
                        {
                            "tedu_microservices_api.read",
                            "tedu_microservices_api.write"
                        }
                    }
                });
            });
        }
    }
}