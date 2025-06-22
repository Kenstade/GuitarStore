using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

internal sealed record RemoveItemRequest(string Id);

internal sealed class RemoveItem : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/cart/{id}", async ([AsParameters]RemoveItemRequest request, CartDbContext dbContext,
            ClaimsPrincipal user, CancellationToken ct) =>
        {
            var parsedRequestId = Guid.Parse(request.Id);
            
            var userId = user.GetUserId();
        
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);
        
            if (cart == null) return Results.Problem(CartErrors.NotFound(userId));
        
            cart.RemoveItem(parsedRequestId);
            await dbContext.SaveChangesAsync(ct);
        
            return Results.Ok();
        })
        .AddEndpointFilter<LoggingEndpointFilter<RemoveItemRequest>>()    
        .AddEndpointFilter<ValidationEndpointFilter<RemoveItemRequest>>()
        .WithTags("Cart")
        .WithName("RemoveItemFromCart")
        .WithSummary("Remove item from cart")
        .WithDescription("Remove item from the current user's shopping cart")
        .RequireAuthorization();
        
        return builder;
    }
}

internal sealed class RemoveItemRequestValidator : AbstractValidator<RemoveItemRequest>
{
    public RemoveItemRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}