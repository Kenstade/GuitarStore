﻿using GuitarStore.Common;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Orders.Features;

internal sealed class GetOrders : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/orders", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AppDbContext dbContext, 
        IUserContextProvider userContext)
    {
        var userId = userContext.GetUserId();
        
        var orders = await dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == userId)
            .Select(o => new GetOrdersResponse
            (
                o.Total,
                o.OrderStatus.ToString(),
                o.CreatedAt,
                o.Items.Select(i => i.Image).ToList()
            )).ToListAsync();

        if (orders == null) return TypedResults.NotFound("Your order list is empty");

        return TypedResults.Ok(orders);

    }
}
public record GetOrdersResponse(decimal Total, string Status, DateTime AddedAt, ICollection<string> Images);
