using GuitarStore.Common.Events;
using GuitarStore.Data;

namespace GuitarStore.Orders.Events;
public sealed record OrderDoneEvent(Guid CartId, IDictionary<int, int> Items) : INotification;

internal sealed class OrderCreatedEventHandler(AppDbContext dbContext, INotifier notifier)
    : INotificationHandler<OrderDoneEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly INotifier _notifier = notifier;

    public async Task Handle(OrderDoneEvent message) 
    {
        var cart = await _dbContext.Carts.FindAsync(message.CartId);
        if(cart != null)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }       

        await _notifier.Send(new CustomerCartRemovedEvent(message.Items));
    }
}