using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Web.MinimalApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Order.Data;

namespace MusicStore.Modules.Order.Features;

internal sealed class GetOrders : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/orders", async (ClaimsPrincipal claims, OrderDbContext dbContext, CancellationToken ct) =>
        {
            var orders = await dbContext.Orders
                .Where(o => o.CustomerId == claims.GetUserId())
                .Select(o => new GetOrdersResponse
                (
                    o.OrderStatus.ToString(),
                    o.Total,
                    o.Items.Select(i => new OrderItemSummaryResponse(i.Image)).ToList()
                )).ToListAsync(ct);
            
            return orders.Any() ? Results.Ok(orders)
                                : Results.Ok("Your order list is empty");   
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetOrders>>()
        .WithTags("Orders")
        .WithName("GetOrders")
        .WithSummary("Get orders")
        .WithDescription("Get current user's orders")
        .RequireAuthorization(Constants.Permissions.GetOrder);

        return builder;
    }
}

internal sealed record GetOrdersResponse(string OrderStatus, decimal Total, ICollection<OrderItemSummaryResponse> Items);
internal sealed record OrderItemSummaryResponse(string? Image);