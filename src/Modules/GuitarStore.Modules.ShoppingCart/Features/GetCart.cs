using BuildingBlocks.Web;
using BuildingBlocks.Web.EndpointFilters;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record GetCartRequest();

public sealed class GetCart : IEndpoint
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
        .WithName("GetCart");    
        
        return builder;
    }

    private async Task<IResult> Handle(CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = await _dbContext.Carts
            .AsNoTracking()
            .Select(c => new {c.CustomerId, c.Items})
            .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);
        
        if (cart == null) return TypedResults.Ok("Your cart is empty");
            
        return TypedResults.Ok();
    }
}

public sealed record GetCartResponse(decimal Total, ICollection<CartItemPartialResponse> Items);
public sealed record CartItemPartialResponse(string Name, string Image, decimal Price, int Quantity);