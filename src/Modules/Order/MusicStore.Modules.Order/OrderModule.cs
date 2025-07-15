using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MusicStore.Modules.Order.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.Order.BackgroundJobs;
using MusicStore.Modules.Order.Data;
using MusicStore.Modules.Order.Events.CancelOrderSaga;
using MusicStore.Modules.Order.Events.CreateOrderSaga;
using MusicStore.Modules.Order.Events.Integration;

namespace MusicStore.Modules.Order;
public static class OrderModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<OrderDbContext>(configuration);


        services.AddEventPublisher(typeof(OrderModule).Assembly);

        services.AddScoped<ProcessOutboxMessagesJob>();
        services.AddValidatorsFromAssembly(typeof(OrderModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(OrderModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseOrdersModule(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseMigration<OrderDbContext>();
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
