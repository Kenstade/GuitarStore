using GuitarStore.Common.Events;
using GuitarStore.Data;

namespace GuitarStore.Checkout.Events;
public sealed record OrderDoneEvent(Guid CartId, IDictionary<int, int> Items) : IEvent;
internal sealed class ReduceStockEventHandler(AppDbContext dbContext) : IEventHandler<OrderDoneEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task Handle(OrderDoneEvent message)
    {
        var products = _dbContext.Products
           .Where(p => message.Items.Keys.Contains(p.Id));

        foreach (var product in products)
        {
            var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;

            product.RemoveStock(productUnits);

            _dbContext.Update(product);
        }
        await _dbContext.SaveChangesAsync();
    }
}
internal sealed class RemoveCartEventHandler(AppDbContext dbContext) : IEventHandler<OrderDoneEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task Handle(OrderDoneEvent message)
    {
        var cart = await _dbContext.Carts.FindAsync(message.CartId);
        if (cart != null)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }
    }
}