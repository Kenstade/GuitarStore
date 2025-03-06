using GuitarStore.Common.Events;
using GuitarStore.Data;

namespace GuitarStore.Orders.Events;

public sealed record CustomerCartRemovedEvent(IDictionary<int, int> Items) : INotification;

internal sealed class CustomerCartRemovedEventHandler(AppDbContext dbContext,
    ILogger<CustomerCartRemovedEventHandler> logger)
    : INotificationHandler<CustomerCartRemovedEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<CustomerCartRemovedEventHandler> _logger = logger;
    public async Task Handle(CustomerCartRemovedEvent message)
    {
        _logger.LogInformation($"Handling {nameof(CustomerCartRemovedEvent)}");

        var products = _dbContext.Products
            .Where(p => message.Items.Keys.Contains(p.Id));

        foreach (var product in products)
        {            
            var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;

            product.Stock -= productUnits;

            if (product.Stock <= 0) product.IsAvailable = false;

            _dbContext.Update(product);           
        }
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"{nameof(CustomerCartRemovedEvent)} completed");
    }   
}
