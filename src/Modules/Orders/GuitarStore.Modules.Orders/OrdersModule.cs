using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Orders.BackgroundJobs;
using GuitarStore.Modules.Orders.Data;
using GuitarStore.Modules.Orders.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Orders;
public static class OrdersModule
{
    public const string ModuleName = "Orders";
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
            
        return configurator;
    }
}
