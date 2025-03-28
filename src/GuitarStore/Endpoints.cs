using GuitarStore.Catalogs.Features;
using GuitarStore.Checkout.Features;
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
           .MapEndpoint<UpdateCustomer>()
           .MapEndpoint<RefreshToken>()
           .MapEndpoint<Logout>();

        app.MapGroup("catalog")
           .WithTags("Catalog")
           .MapEndpoint<GetCatalog>()
           .MapEndpoint<GetProductDetails>();

        app.MapGroup("cart")
            .WithTags("ShoppingCart")
            .MapEndpoint<GetCart>()
            .MapEndpoint<AddItemToCart>();

        app.MapGroup("checkout")
           .WithTags("Checkout")
           .MapEndpoint<AddAddress>()
           .MapEndpoint<CreateOrder>();

        app.MapGroup("orders")
           .WithTags("Orders")
           .MapEndpoint<GetOrders>()
           .MapEndpoint<GetOrderDetails>();
    }
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
