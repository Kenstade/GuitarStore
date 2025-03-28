﻿using GuitarStore.Modules.Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;

namespace GuitarStore.Modules.Catalog;
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CatalogDbContext>(configuration);
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);

        services.AddMinimalApiEndpoints(typeof(CatalogModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.UseMigration<CatalogDbContext>();
        app.UseEndpoints(endpoints => endpoints.MapEndpoints());
        return app;
    }
}
