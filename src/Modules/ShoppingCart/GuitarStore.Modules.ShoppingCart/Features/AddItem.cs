using BuildingBlocks.Web;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Catalog.Contracts;
using GuitarStore.Modules.ShoppingCart.Data;
using GuitarStore.Modules.ShoppingCart.Errors;
using GuitarStore.Modules.ShoppingCart.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GuitarStore.Modules.ShoppingCart.Features;

public sealed record AddItemRequest(string ProductId);

internal sealed class AddItem : IEndpoint
{
    private readonly CartDbContext _dbContext;
    private readonly IUserContextProvider _userContext;
    private readonly ICatalogService _catalogService;

    public AddItem(CartDbContext dbContext, IUserContextProvider userContext, ICatalogService catalogService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _catalogService = catalogService;
    }
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("cart/add", async (AddItemRequest request, CancellationToken ct) =>
        {
            var parsedRequest = Guid.Parse(request.ProductId);
            
            return await Handle(parsedRequest, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<AddItemRequest>>()   
        .AddEndpointFilter<ValidationEndpointFilter<AddItemRequest>>()
        .WithName("AddItemIntoCart")
        .RequireAuthorization();
        
        return builder;
    }

    private async Task<IResult> Handle(Guid requestId, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        
        var cart = _dbContext.Carts
            .FirstOrDefault(c => c.CustomerId == userId); //придумать как достать из кеша (dbConextExtension?)

        if (cart == null)
        {
            cart = Cart.Create(userId);
            await _dbContext.Carts.AddAsync(cart);
        }

        var product = await _catalogService.GetProductForCartAsync(requestId, ct);
        if (product == null) 
            return TypedResults.Problem(new ProductNotFoundError(requestId));
        
        cart.AddItem(requestId, product.Name, product.Image, product.Price);
        
        await _dbContext.SaveChangesAsync();
        
        return TypedResults.Ok($"{requestId}");
    }
}

public sealed class AddItemRequestValidator : AbstractValidator<AddItemRequest>
{
    public AddItemRequestValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}