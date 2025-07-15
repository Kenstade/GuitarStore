using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.Customer.Data;
using MusicStore.Modules.Customer.Events.Integration;

namespace MusicStore.Modules.Customer;
public static class CustomerModule
{
    public static IServiceCollection AddCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CustomerDbContext>(configuration);

        services.AddValidatorsFromAssembly(typeof(CustomerModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(CustomerModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCustomerModule(this IApplicationBuilder app)
    {
        app.UseMigration<CustomerDbContext>();

        return app;
    }
    
    public static IRegistrationConfigurator AddCustomerModuleConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<UserRegisteredConsumer>();
        configurator.AddConsumer<OrderStatusChangedToAwaitingValidationConsumer>();
        
        return configurator;
    }
}
