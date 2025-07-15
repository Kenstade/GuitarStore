using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.ShoppingCart.Data;
using MusicStore.Modules.ShoppingCart.Events.Integration;

namespace MusicStore.Modules.ShoppingCart;
public static class ShoppingCartModule
{
    public static IServiceCollection AddShoppingCartModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CartsDbContext>(configuration);
        services.AddValidatorsFromAssembly(typeof(ShoppingCartModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(ShoppingCartModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseShoppingCartModule(this IApplicationBuilder app)
    {
        app.UseMigration<CartsDbContext>();
        return app;
    }
    
    public static IRegistrationConfigurator AddShoppingCartModuleConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<OrderPlacedConsumer>();
        
        return configurator;
    }
}
