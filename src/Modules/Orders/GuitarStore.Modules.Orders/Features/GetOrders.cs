using System.Security.Claims;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Security.Authentication;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Modules.Orders.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Orders.Features;

internal sealed class GetOrders : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/orders", async (ClaimsPrincipal claims, OrdersDbContext dbContext, CancellationToken ct) =>
        {
            var orders = await dbContext.Orders
                .Where(o => o.CustomerId == claims.GetUserId())
                .Select(o => new GetOrdersResponse
                (
                    o.OrderStatus.ToString(),
                    o.Items.Select(i => new OrderItemSummaryResponse
                    (
                        i.Id,
                        i.Image
                    )).ToList()
                )).ToListAsync(ct);
            
            return orders.Any() ? Results.Ok(orders)
                                : Results.Ok("Your order list is empty");   
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetOrders>>()
        .WithName("GetOrders")
        .WithTags("Orders")
        .RequireAuthorization(Constants.Permissions.GetOrder);

        return builder;
    }
}

public sealed record GetOrdersResponse(string OrderStatus, ICollection<OrderItemSummaryResponse> Items);
public sealed record OrderItemSummaryResponse(int Id, string? Image);