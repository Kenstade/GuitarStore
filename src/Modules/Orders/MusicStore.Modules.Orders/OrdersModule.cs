using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MusicStore.Modules.Orders.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.Orders.BackgroundJobs;
using MusicStore.Modules.Orders.Data;
using MusicStore.Modules.Orders.Events.CancelOrderSaga;
using MusicStore.Modules.Orders.Events.CreateOrderSaga;
using MusicStore.Modules.Orders.Events.Integration;

namespace MusicStore.Modules.Orders;
public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<OrdersDbContext>(configuration);


        services.AddEventPublisher(typeof(OrdersModule).Assembly);

        services.AddScoped<ProcessOutboxMessagesJob>();
        services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(OrdersModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseOrdersModule(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseMigration<OrdersDbContext>();
        app.UseBackgroundJobs(configuration);
        
        return app;
    }
    
    public static IRegistrationConfigurator AddOrdersModuleConsumers(this IRegistrationConfigurator configurator, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetOptions<RedisOptions>(nameof(RedisOptions)).ConnectionString;
        
        configurator.AddConsumer<CartItemsReceivedConsumer>();
        configurator.AddConsumer<OrderStockConfirmationFailedConsumer>();
        configurator.AddConsumer<CustomerAddressConfirmedConsumer>();
        configurator.AddConsumer<AddressConfirmationFailedConsumer>();
        configurator.AddConsumer<OrderValidatedConsumer>();

        configurator.AddSagaStateMachine<CancelOrderSaga, CancelOrderState>()
            .InMemoryRepository();
        
        configurator.AddSagaStateMachine<CreateOrderSaga, CreateOrderState>()
            .InMemoryRepository();
            
        return configurator;
    }
}
