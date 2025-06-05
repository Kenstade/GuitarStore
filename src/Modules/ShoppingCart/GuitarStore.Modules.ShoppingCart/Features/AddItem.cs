using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Catalog.Contracts;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record AddItemRequest(string Id);

internal sealed class AddItem : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("cart/add-item", async (AddItemRequest request, CartDbContext dbContext, 
                IUserContext userContext, ICatalogService catalogService, CancellationToken ct) =>
        {
            var parsedRequestId = Guid.Parse(request.Id); 
            
            var userId = userContext.GetUserId();
        
            var cart = dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.CustomerId == userId);

            if (cart == null)
            {
                cart = Cart.Create(userId);
                await dbContext.Carts.AddAsync(cart, ct);
            }

            var result = await catalogService.GetProductForCartAsync(parsedRequestId, ct);
            
            if (result.IsFailure)
            {
                return Results.Problem(result.Error);
            }

            cart.AddItem(parsedRequestId, result.Value.Name, result.Value.Image, result.Value.Price);

            await dbContext.SaveChangesAsync(ct);
        
            return Results.Ok($"{parsedRequestId}");
        })
        .AddEndpointFilter<LoggingEndpointFilter<AddItemRequest>>()   
        .AddEndpointFilter<ValidationEndpointFilter<AddItemRequest>>()
        .WithName("AddItemIntoCart")
        .WithTags("Cart")
        .RequireAuthorization();
        
        return builder;
    }
}

public sealed class AddItemRequestValidator : AbstractValidator<AddItemRequest>
{
    public AddItemRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}