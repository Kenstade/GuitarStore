using BuildingBlocks.Web;
using BuildingBlocks.Web.EndpointFilters;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record AddItemToCartRequest(Guid ProductId);

public sealed class AddItemToCart : IEndpoint
{
    private readonly CartDbContext _dbContext;
    private readonly IUserContextProvider _userContext;

    public AddItemToCart(CartDbContext dbContext, IUserContextProvider userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("cart/add", async ([FromBody]AddItemToCartRequest request,CancellationToken ct) =>
        {
            return await Handle(request, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<AddItemToCartRequest>>()    
        .WithName("AddItemToCart");
        
        return builder;
    }

    private async Task<IResult> Handle(AddItemToCartRequest request,CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.CustomerId == userId);

        if (cart == null)
        {
            cart = Cart.Create(userId);
            await _dbContext.Carts.AddAsync(cart);
        }
        
        
        return TypedResults.Ok();
    }
}