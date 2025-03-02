using GuitarStore.Catalogs.Features;
using GuitarStore.Common.Web;
using GuitarStore.Customers.Features;
using GuitarStore.Identity.Features;
using GuitarStore.Orders.Features;
using GuitarStore.ShoppingCart.Features;

namespace GuitarStore;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGroup("account")
           .WithTags("Account")
           .MapEndpoint<Register>()
           .MapEndpoint<Login>()
           .MapEndpoint<RefreshToken>()
           .MapEndpoint<Logout>();

        app.MapGroup("catalog")
           .WithTags("Catalog")
           .MapEndpoint<GetCatalog>()
           .MapEndpoint<GetProductById>();

        app.MapGroup("cart")
            .WithTags("ShoppingCart")
            .MapEndpoint<GetCart>()
            .MapEndpoint<AddItemToCart>();

        app.MapGroup("checkout")
           .WithTags("Checkout")
           .MapEndpoint<AddAddress>()
           .MapEndpoint<UpdateAddress>()
           .MapEndpoint<CreateOrder>();

        app.MapGroup("orders")
           .WithTags("Orders")
           .MapEndpoint<GetOrders>();
    }
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
