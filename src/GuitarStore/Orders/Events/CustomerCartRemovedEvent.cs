using GuitarStore.Common.Events;
using GuitarStore.Data;

namespace GuitarStore.Orders.Events;

public sealed record CustomerCartRemovedEvent(IDictionary<int, int> Items) : INotification;

internal sealed class CustomerCartRemovedEventHandler(AppDbContext dbContext)
    : INotificationHandler<CustomerCartRemovedEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task Handle(CustomerCartRemovedEvent message)
    {
        var products = _dbContext.Products
            .Where(p => message.Items.Keys.Contains(p.Id));

        foreach (var product in products)
        {            
            var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;
            //if product.Stock < productUnits throw exception 

            product.Stock -= productUnits;

            if (product.Stock == 0) product.IsAvailable = false;

            _dbContext.Update(product);           
        }
        await _dbContext.SaveChangesAsync();
    }   
}
