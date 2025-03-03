using GuitarStore.Data;

namespace GuitarStore.Orders.Events;

public sealed record OrderCreatedEvent(Guid CartId, IDictionary<int, int> Items);
internal sealed class OrderEventHandler(OrderCreatedEventHandler orderCreatedEventHandler)
{
    private readonly OrderCreatedEventHandler _orderCreatedEventHandler = orderCreatedEventHandler;
    public Task Handle(OrderCreatedEvent orderCreatedEvent)
    { 
        return _orderCreatedEventHandler.Handle(orderCreatedEvent);
    }
}

internal class OrderCreatedEventHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    public OrderCreatedEventHandler(AppDbContext dbContext, ILogger<OrderCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task Handle(OrderCreatedEvent message)
    {
        _logger.LogInformation($"Handling {nameof(OrderCreatedEvent)}");
        //await _dbContext.Carts.Where(c => c.Id == orderCreatedEvent.CartId).ExecuteDeleteAsync()
        //doesn't work with InMemory db
        var cart = await _dbContext.Carts.FindAsync(message.CartId);
        if(cart != null)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }

        var products = _dbContext.Products
            .Where(p => message.Items.Keys.Contains(p.Id));

        foreach (var product in products)
        {
            var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;

            product.Stock -= productUnits;
            _dbContext.Update(product);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"{nameof(OrderCreatedEvent)} completed");
    }
}