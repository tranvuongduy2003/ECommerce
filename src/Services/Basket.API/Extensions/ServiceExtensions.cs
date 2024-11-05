using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Contracts.Services;
using EventBus.Messages.Events;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Inventory.Grpc.Protos;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurationSettings(configuration);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        services.ConfigureRedis();
        services.AddInfrastructureService();
        services.ConfigureMassTransit();
        services.ConfigureGrpcService();
        services.ConfigureHealthChecks();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>();
        services.AddSingleton(eventBusSettings);

        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        services.AddSingleton(cacheSettings);

        var urlSettings = configuration.GetSection(nameof(UrlSettings)).Get<UrlSettings>();
        services.AddSingleton(urlSettings);
    }

    private static void ConfigureRedis(this IServiceCollection services)
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(settings.ConnectionString))
        {
            throw new ArgumentException("Redis Conenction string is not configured!");
        }

        services.AddStackExchangeRedisCache(options => { options.Configuration = settings.ConnectionString; });
    }

    private static void AddInfrastructureService(this IServiceCollection services)
    {
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializerService, SerializerService>()
            .AddScoped<IEmailTemplateService, EmailTemplateService>()
            .AddScoped<IBasketEmailService, BasketEmailService>();
    }

    private static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
        {
            throw new ArgumentException("EventBusSettings is not configured!");
        }

        var mqConnection = new Uri(settings.HostAddress);

        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) => { cfg.Host(mqConnection); });

            // Publish submit order message, instead of sending it to a specific queue directly.
            config.AddRequestClient<IBasketCheckoutEvent>();
        });
    }

    private static void ConfigureGrpcService(this IServiceCollection services)
    {
        var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
        services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x => x.Address = new Uri(settings.StockUrl));
        services.AddScoped<StockItemGrpcService>();
    }
    
    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        services.AddHealthChecks().AddRedis(settings.ConnectionString, "Redis Health", HealthStatus.Degraded);
    }
}