using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Customers.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Customers;
public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CustomersDbContext>(configuration);

        services.AddValidatorsFromAssembly(typeof(CustomersModule).Assembly);
        services.AddMinimalApiEndpoints(typeof(CustomersModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCustomersModule(this IApplicationBuilder app)
    {
        app.UseMigration<CustomersDbContext>();

        return app;
    }
}
