using BuildingBlocks.Core.Security;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record GetCartRequest;

internal sealed class GetCart : IEndpoint
{
    private readonly CartDbContext _dbContext;
    private readonly IUserContextProvider _userContext;

    public GetCart(CartDbContext dbContext, IUserContextProvider userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/cart", async (CancellationToken ct) =>
        {
            return await Handle(ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetCartRequest>>()    
        .WithName("GetCart")
        .WithTags("Cart")
        .RequireAuthorization();    
        
        return builder;
    }

    private async Task<IResult> Handle(CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = await _dbContext.Carts
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
        
        if (cart == null) return TypedResults.Ok("Your cart is empty");
            
        return TypedResults.Ok(cart);
    }
}

public sealed record GetCartResponse(decimal Total, ICollection<CartItemPartialResponse> Items);
public sealed record CartItemPartialResponse(string Name, string? Image, decimal Price, int Quantity);