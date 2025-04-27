using BuildingBlocks.Core.Security;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record RemoveItemRequest(Guid ProductId);

internal sealed class RemoveItem : IEndpoint
{
    private readonly CartDbContext _dbContext;
    private readonly IUserContextProvider _userContext;

    public RemoveItem(CartDbContext dbContext, IUserContextProvider userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/cart/remove/{id}", async ([AsParameters]RemoveItemRequest request, CancellationToken ct) =>
        {
            return await Handle(request, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<RemoveItemRequest>>()    
        .WithName("RemoveItemFromCart")
        .WithTags("Cart")
        .RequireAuthorization();
        
        return builder;
    }

    private async Task<IResult> Handle(RemoveItemRequest request, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);
        
        if (cart == null) return TypedResults.Problem(new CartNotFoundError(userId));
        
        cart.RemoveItem(request.ProductId);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }
}