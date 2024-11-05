using Common.Logging;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICheckoutService, CheckoutService>();
        services.AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>();
        services.AddTransient<LoggingDelegatingHandler>();
    }

    public static void ConfigureHttpRepository(this IServiceCollection services)
    {
        services.AddScoped<IBasketHttpRepository, BasketHttpRepository>();
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();
        services.AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
    }

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureBasketHttpClients(services);
        ConfigureOrderHttpClients(services);
        ConfigureInventoryHttpClients(services);
    }

    private static void ConfigureBasketHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketAPI",
                (sp, cl) => { cl.BaseAddress = new Uri("http://localhost:5004/api/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();
        //services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketAPI"));
    }

    private static void ConfigureOrderHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrderAPI",
                (sp, cl) => { cl.BaseAddress = new Uri("http://localhost:5005/api/v1/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();
        //services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrderAPI"));
    }

    private static void ConfigureInventoryHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI",
                (sp, cl) => { cl.BaseAddress = new Uri("http://localhost:5006/api/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();
        //services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }
}