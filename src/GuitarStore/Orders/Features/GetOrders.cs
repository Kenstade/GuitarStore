using GuitarStore.Common.Web;
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
            .Where(o => o.CustomerId == userId)
            .Select(o => new GetOrdersResponse
            (
                o.Total,
                o.OrderStatus.ToString(),
                o.CreatedAt,
                o.Items.Select(i => new OrderItemsPartialResponse(i.Name, i.Image)).ToList()
            )).ToListAsync();

        if (orders == null) return TypedResults.NotFound("Your order list is empty");

        return TypedResults.Ok(orders);

    }
}
public sealed record GetOrdersResponse(
    decimal Total, 
    string Status, 
    DateTime CreatedAt,
    ICollection<OrderItemsPartialResponse> Items);
public sealed record OrderItemsPartialResponse(string Name, string Image);
