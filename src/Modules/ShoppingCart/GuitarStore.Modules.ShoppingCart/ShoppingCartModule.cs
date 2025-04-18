using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.ShoppingCart;
public static class ShoppingCartModule
{
    public static IServiceCollection AddShoppingCartModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CartDbContext>(configuration);
        
        services.AddValidatorsFromAssembly(typeof(ShoppingCartModule).Assembly, includeInternalTypes: true);
        
        services.AddMinimalApiEndpoints(typeof(ShoppingCartModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseShoppingCartModule(this IApplicationBuilder app)
    {
        app.UseMigration<CartDbContext>();
        return app;
    }
}
