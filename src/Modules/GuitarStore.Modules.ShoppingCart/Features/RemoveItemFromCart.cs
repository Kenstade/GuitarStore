using BuildingBlocks.Web;
using BuildingBlocks.Web.EndpointFilters;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record RemoveItemFromCartRequest(Guid ProductId);

public class RemoveItemFromCart : IEndpoint
{
    private readonly CartDbContext _dbContext;
    private readonly IUserContextProvider _userContext;

    public RemoveItemFromCart(CartDbContext dbContext, IUserContextProvider userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/cart/remove/{id}", async ([AsParameters]RemoveItemFromCartRequest request, CancellationToken ct) =>
        {
            return await Handle(request, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<AddItemToCartRequest>>()    
        .WithName("RemoveItemFromCart");
        
        return builder;
    }

    private async Task<IResult> Handle(RemoveItemFromCartRequest request, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);

        //throw ex?
        if (cart == null) return TypedResults.Problem(new CartNotFoundError(userId));
        
        cart.RemoveItem(request.ProductId);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }
}