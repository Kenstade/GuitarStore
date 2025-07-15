using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MusicStore.Modules.Order.Data;
using MusicStore.Modules.Order.Models;

namespace MusicStore.Modules.Order.Features;

internal sealed record CreateOrderRequest(string AddressId);
internal sealed class CreateOrder : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/orders", async (CreateOrderRequest request, OrderDbContext dbContext, ClaimsPrincipal claims, 
            CancellationToken ct) =>
        {
            var order = Models.Order.Create(claims.GetUserId(), Guid.Parse(request.AddressId));
            
            await dbContext.AddAsync(order, ct);
            await dbContext.SaveChangesAsync(ct);
            
            return Results.Ok($"Order '{order.Id}' placed.");
        })
        .WithTags("Orders")
        .WithName("CreateOrder")
        .WithSummary("Checkout")
        .WithDescription("Create a new order for the current user")
        .AddEndpointFilter<LoggingEndpointFilter<CreateOrder>>()
        .AddEndpointFilter<ValidationEndpointFilter<CreateOrderRequest>>()
        .RequireAuthorization(Constants.Permissions.CreateOrder);    
        
        return builder;
    }
}

internal sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(p => p.AddressId).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}