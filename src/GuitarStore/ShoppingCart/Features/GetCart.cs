using GuitarStore.Common.Web;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.ShoppingCart.Features;

internal sealed class GetCart : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AppDbContext dbContext, 
        IUserContextProvider userContext)
    {
        var userId = userContext.GetUserId();

        var cart = await dbContext.Carts
            .AsNoTracking()
            .Select(c => new {c.CustomerId, c.Items})            
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null) return TypedResults.Ok("Your cart is empty");

        var products = await dbContext.Products
            .AsNoTracking()
            .Where(p => cart.Items.Select(c => c.ProductId).Contains(p.Id) && p.IsAvailable)
            .ToListAsync();

       string message = string.Empty;
        var items = cart.Items
            .Select(cartItem =>
            {
                var product = products.First(p => p.Id == cartItem.ProductId);
                //TODO: переделать?
                if(product.Stock < cartItem.Quantity) message = "Out of stock";
                
                return new CartItemPartialResponse(product.Name, product.Image, cartItem.Price, cartItem.Quantity, message);
            }).ToList();

        var total = items.Select(i => i.Quantity * i.Price).Sum();

        return TypedResults.Ok(new GetCartResponse(total, items));
    }
}
public sealed record GetCartResponse(decimal Total, ICollection<CartItemPartialResponse> Items);
public sealed record CartItemPartialResponse(
    string Name, 
    string Image, 
    decimal Price, 
    int Quantity, 
    string Message);
