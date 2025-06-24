using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Orders.Data;
using MusicStore.Modules.Orders.Errors;

namespace MusicStore.Modules.Orders.Features;

internal sealed record CancelOrderRequest(string Id);

internal sealed class CancelOrder : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/orders/cancel/{id}", async ([AsParameters]CancelOrderRequest request, 
                OrdersDbContext dbContext, CancellationToken ct) =>
        {
            var order = await dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(request.Id), ct);

            if (order is null) return Results.Problem(OrderErrors.NotFound(request.Id));

            order.SetCancelledStatus();
            
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok();
        })
        .AddEndpointFilter<LoggingEndpointFilter<CancelOrder>>()
        .AddEndpointFilter<ValidationEndpointFilter<CancelOrderRequest>>()
        .WithTags("Orders")
        .WithName("CancelOrder")
        .WithSummary("Cancel order by ID")
        .WithDescription("Cancel the current user's order by ID")
        .RequireAuthorization(Constants.Permissions.UpdateOrder);

        return builder;
    }
}

internal sealed class CancelOrderRequestValidator : AbstractValidator<CancelOrderRequest>
{
    public CancelOrderRequestValidator()
    {
        RuleFor(o => o.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}