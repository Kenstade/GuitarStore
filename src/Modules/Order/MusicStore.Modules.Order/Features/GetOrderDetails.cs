using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Order.Data;
using MusicStore.Modules.Order.Errors;

namespace MusicStore.Modules.Order.Features;

internal sealed record GetOrderDetailsRequest(string Id);

internal sealed class GetOrderDetails : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/orders/{id}", async ([AsParameters]GetOrderDetailsRequest request, OrderDbContext dbContext, 
            ClaimsPrincipal user, CancellationToken ct) =>
        {
            var order = await dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Id == Guid.Parse(request.Id) && o.CustomerId == user.GetUserId())
                .Select(o => new GetOrderDetailsResponse
                (
                    o.OrderStatus.ToString(),
                    o.Address.City,
                    o.Address.Street,
                    o.Address.BuildingNumber,
                    o.Address.Apartment,
                    o.Items.Select(i => new OrderItemSummary
                    (
                        i.Name,
                        i.Price,
                        i.Image,
                        i.Quantity
                    )).ToList()
                )).FirstOrDefaultAsync(ct);
            
            return order is not null 
                ? Results.Ok(order)
                : Results.Problem(OrderErrors.NotFound(request.Id));
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetOrderDetails>>()
        .AddEndpointFilter<ValidationEndpointFilter<GetOrderDetailsRequest>>()
        .WithTags("Orders")
        .WithName("GetOrderDetails")
        .WithSummary("Get order details")
        .WithDescription("Get the current user's order by ID")
        .RequireAuthorization(Constants.Permissions.GetOrder);

        return builder;
    }
}

internal sealed record GetOrderDetailsResponse(
    string OrderStatus, 
    string City, 
    string Street, 
    string BuildingNumber, 
    string Apartment, 
    ICollection<OrderItemSummary> Items);

internal sealed record OrderItemSummary(string Name, decimal Price, string? Image, int Quantity);

internal sealed class GetOrderDetailsRequestValidator : AbstractValidator<GetOrderDetailsRequest>
{
    public GetOrderDetailsRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}
