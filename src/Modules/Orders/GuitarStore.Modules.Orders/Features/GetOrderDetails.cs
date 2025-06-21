using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Orders.Data;
using GuitarStore.Modules.Orders.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Orders.Features;

public sealed record GetOrderDetailsRequest(string Id);

internal sealed class GetOrderDetails : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/orders/{id}", async ([AsParameters]GetOrderDetailsRequest request, OrdersDbContext dbContext, 
            CancellationToken ct) =>
        {
            var order = await dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Id == Guid.Parse(request.Id))
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
        .WithName("GetOrderDetails")
        .WithTags("Orders")
        .RequireAuthorization(Constants.Permissions.GetOrder);

        return builder;
    }
}

public sealed record GetOrderDetailsResponse(
    string OrderStatus, 
    string City, 
    string Street, 
    string BuildingNumber, 
    string Apartment, 
    ICollection<OrderItemSummary> Items);

public sealed record OrderItemSummary(string Name, decimal Price, string? Image, int Quantity);

public sealed class GetOrderDetailsRequestValidator : AbstractValidator<GetOrderDetailsRequest>
{
    public GetOrderDetailsRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}
