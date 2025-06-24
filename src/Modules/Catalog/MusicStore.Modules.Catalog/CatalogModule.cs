using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Web.MinimalApi;
using MassTransit;
using MusicStore.Modules.Catalog.Contracts;
using MusicStore.Modules.Catalog.Data;
using MusicStore.Modules.Catalog.Events.Integration;

namespace MusicStore.Modules.Catalog;
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
        configurator.AddConsumer<OrderCancelledConsumer>();
        
        return configurator;
    }
    
}
