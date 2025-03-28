using GuitarStore.Common.Web;
using GuitarStore.Data;
using GuitarStore.Orders.Errors;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Orders.Features;

public sealed record GetOrderByIdRequest(Guid Id);
internal sealed class GetOrderDetails : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync([AsParameters]GetOrderByIdRequest request, AppDbContext dbContext, 
        IUserContextProvider userContext)
    { 
        var userId = userContext.GetUserId();
        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == userId && o.Id == request.Id)
            .Select(o => new GetOrderByIdResponse
            (
                o.Total,
                o.OrderStatus.ToString(),
                o.CreatedAt,
                o.Items.Select(i => new OrderItemPartialResponse
                (
                    i.Name,
                    i.Image,
                    i.Price,
                    i.Quantity
                )).ToList()
            )).FirstOrDefaultAsync();

        return order != null ? TypedResults.Ok(order)
                             : TypedResults.Problem(new OrderNotFoundError(request.Id));
    }
}
public sealed record GetOrderByIdResponse(
    decimal Total, 
    string Status, 
    DateTime CreatedAt, 
    IEnumerable<OrderItemPartialResponse> Items);
public sealed record OrderItemPartialResponse(string Name, string Image, decimal Price, int Quantity);