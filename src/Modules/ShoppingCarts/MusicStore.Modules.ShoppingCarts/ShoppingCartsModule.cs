using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.ShoppingCarts.Data;
using MusicStore.Modules.ShoppingCarts.Events.Integration;

namespace MusicStore.Modules.ShoppingCarts;
public static class ShoppingCartsModule
{
    public static IServiceCollection AddShoppingCartsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CartsDbContext>(configuration);
        services.AddValidatorsFromAssembly(typeof(ShoppingCartsModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(ShoppingCartsModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseShoppingCartsModule(this IApplicationBuilder app)
    {
        app.UseMigration<CartsDbContext>();
        return app;
    }
    
    public static IRegistrationConfigurator AddShoppingCartsModuleConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<OrderPlacedConsumer>();
        
        return configurator;
    }
}
