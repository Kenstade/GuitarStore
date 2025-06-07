using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Catalog.Contracts;
using GuitarStore.Modules.Catalog.Events.Integration;
using MassTransit;

namespace GuitarStore.Modules.Catalog;
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CatalogDbContext>(configuration);
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        services.AddScoped<ICatalogService, CatalogService>();
        
        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(CatalogModule).Assembly);
        services.AddEventPublisher(typeof(CatalogModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseMigration<CatalogDbContext>();
        return app;
    }
    
    public static IRegistrationConfigurator AddCatalogModuleConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<OrderStatusChangedToAwaitingValidationConsumer>();
        configurator.AddConsumer<OrderStatusChangedToCancelledConsumer>();
        
        return configurator;
    }
    
}
