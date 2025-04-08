using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Catalog.Contracts;

namespace GuitarStore.Modules.Catalog;
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CatalogDbContext>(configuration);
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        services.AddScoped<ICatalogService, CatalogService>();
        
        services.AddEvents(typeof(CatalogModule).Assembly);
        
        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);

        services.AddMinimalApiEndpoints(typeof(CatalogModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.UseMigration<CatalogDbContext>();
        return app;
    }
}
