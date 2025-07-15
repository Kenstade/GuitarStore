using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security;
using BuildingBlocks.Web.MinimalApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.ShoppingCart.Data;

namespace MusicStore.Modules.ShoppingCart.Features;

internal sealed class GetCart : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/cart", async (CartsDbContext dbContext, IUserContext userContext, CancellationToken ct) =>
        {
            var userId = userContext.GetUserId();
        
            var cart = await dbContext.Carts
                .AsNoTracking()
                .Where(c => c.CustomerId == userId)
                .Select(c => new GetCartResponse
                (
                    c.Items.Sum(i => i.Quantity * i.Price),
                    c.Items.Select(i => new CartItemPartialResponse
                    (
                        i.Name, 
                        i.Image, 
                        i.Price, 
                        i.Quantity
                    )).ToList()
                )).FirstOrDefaultAsync(ct);
        
            return cart == null ? Results.Ok("Your cart is empty") 
                                : Results.Ok(cart);
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCart>>()   
        .WithTags("Cart")
        .WithName("GetCart")
        .WithSummary("Get cart")
        .WithDescription("Get the current user's shopping cart")
        .RequireAuthorization();    
        
        return builder;
    }
}

internal sealed record GetCartResponse(decimal Total, ICollection<CartItemPartialResponse> Items);
internal sealed record CartItemPartialResponse(string Name, string? Image, decimal Price, int Quantity);