using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.ShoppingCarts;
public static class ShoppingCartsModuleConfiguration
{
    public static IServiceCollection AddShoppingCartsModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
