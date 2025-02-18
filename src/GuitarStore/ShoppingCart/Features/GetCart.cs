using GuitarStore.Common;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.ShoppingCart.Features;

internal sealed class GetCart : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", HandleAsync)
        .RequireAuthorization();
    private static async Task<GetCartResponse> HandleAsync(AppDbContext dbContext, 
        IUserContextProvider userContext)
    {
        var userId = userContext.GetUserId();

        var cart = await dbContext.Carts
            .Include(c => c.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null) return new GetCartResponse(0, []);

        var products = await dbContext.Products
            .AsNoTracking()
            .Where(p => cart.Items.Select(c => c.ProductId).Contains(p.Id))
            .ToListAsync();

       string message = string.Empty;
        var items = cart.Items
            .Select(cartItem =>
            {
                var product = products.First(p => p.Id == cartItem.ProductId);

                if(product.Stock == 0) message = "Out of stock";
                
                return new CartItemPartialResponse(product.Name, product.Image, cartItem.Price, cartItem.Quantity, message);
            }).ToList();

        var total = items.Select(i => i.Quantity * i.Price).Sum();

        return new GetCartResponse(total, items);
        //TODO: return with Stock if < 10
    }
}
public record GetCartResponse(decimal Total, ICollection<CartItemPartialResponse> Items);
public record CartItemPartialResponse(string Name, string Image, decimal Price, int Quantity, string Message);
