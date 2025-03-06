using GuitarStore.Common.Events;
using GuitarStore.Common.Web;
using GuitarStore.Data;
using GuitarStore.Orders.Events;
using GuitarStore.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Orders.Features;

internal sealed class CreateOrder : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AppDbContext dbContext, 
        IUserContextProvider userContext, INotifier notifier)
    {
        var userId = userContext.GetUserId();

        var cart = await dbContext.Carts
            .AsNoTracking()
            .Select(c => new {c.Id, c.CustomerId, c.Items})
            .FirstOrDefaultAsync(c => c.CustomerId == userId);
        if (cart == null) return TypedResults.BadRequest();

        var address = await dbContext.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CustomerId == userId);
        if (address == null) return TypedResults.BadRequest("Адрес не указан");

        var products = await dbContext.Products
            .AsNoTracking()
            .Where(p => cart.Items.Select(c => c.ProductId).Contains(p.Id) && p.IsAvailable)
            .Select(p => new {p.Id, p.Name, p.Image, p.Stock})
            .ToListAsync();

        var orderItems = cart.Items
            .Select(cartItem =>
            {
                var product = products.First(p => p.Id == cartItem.ProductId);
                return new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Name = product.Name,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    Image = product.Image
                };
            }).ToList();

        await dbContext.Orders.AddAsync(new Order
        {
            Total = orderItems.Select(oi => oi.Price * oi.Quantity).Sum(),
            CustomerId = userId,
            OrderStatus = OrderStatus.Placed,
            CreatedAt = DateTime.UtcNow,   
            Items = orderItems,
            Address = new Address
            {
                City = address.City,
                Street = address.Street,
                BuildingNumber = address.BuildingNumber,
                Apartment = address.Apartment,
            }
        });

        await dbContext.SaveChangesAsync();

        await notifier.Send(new OrderCreatedEvent(
            cart.Id, 
            orderItems.ToDictionary(x => x.ProductId, x => x.Quantity)));

        return TypedResults.Ok();
    }
}
