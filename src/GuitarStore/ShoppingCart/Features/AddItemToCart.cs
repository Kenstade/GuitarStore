using GuitarStore.Common.Web;
using GuitarStore.Data;
using GuitarStore.ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.ShoppingCart.Features;

public sealed record AddItemRequest(int ProductId);
internal sealed class AddItemToCart : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/add-item", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AddItemRequest request, AppDbContext dbContext,
        IUserContextProvider userContext)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);  
        if (product == null) return TypedResults.BadRequest("Item not found");

        var userId = userContext.GetUserId();

        var cart = await dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null)
        {
            cart = new Cart { CustomerId = userId };
            await dbContext.Carts.AddAsync(cart);
        }

        cart.AddItem(request.ProductId, product.Price);
        dbContext.Update(cart.Items.First(x => x.ProductId == request.ProductId));

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
