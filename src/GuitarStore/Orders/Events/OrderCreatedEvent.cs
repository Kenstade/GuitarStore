using GuitarStore.Common.Events;
using GuitarStore.Data;

namespace GuitarStore.Orders.Events;
public sealed record OrderCreatedEvent(Guid CartId, IDictionary<int, int> Items) : INotification;

internal sealed class OrderCreatedEventHandler(AppDbContext dbContext, 
    ILogger<OrderCreatedEventHandler> logger, INotifier notifier)
    : INotificationHandler<OrderCreatedEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<OrderCreatedEventHandler> _logger = logger;
    private readonly INotifier _notifier = notifier;

    public async Task Handle(OrderCreatedEvent message) 
    {
        _logger.LogInformation($"Handling {nameof(OrderCreatedEvent)}");

        var cart = await _dbContext.Carts.FindAsync(message.CartId);
        if(cart != null)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }       
        _logger.LogInformation($"{nameof(OrderCreatedEvent)} completed");

        await notifier.Send(new CustomerCartRemovedEvent(message.Items));
    }
}