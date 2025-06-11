using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;
using GuitarStore.Modules.ShoppingCart.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.ShoppingCart.Events.Integration;

internal sealed class OrderPlacedConsumer(
    CartDbContext dbContext, 
    IBus bus, 
    ILogger<OrderPlacedConsumer> logger) 
    : IConsumer<OrderPlacedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        logger.LogInformation("Handling '{IntegrationEvent}' - '{IntegrationEventId}'.", 
            nameof(OrderPlacedIntegrationEvent), context.Message.Id); 
        
        var integrationEvent = await dbContext.Carts
            .Where(c => c.CustomerId == context.Message.CustomerId)
            .Select(c => new CartItemsReceivedIntegrationEvent
            (
                context.Message.OrderId,
                c.Items.Select(i => new CartItemSummary
                (
                   i.ProductId,
                   i.Name,
                   i.Image,
                   i.Quantity,
                   i.Price
                )).ToList()
            )).FirstOrDefaultAsync();

        if (integrationEvent is null) throw new NotImplementedException(); 
        
        await bus.Publish(integrationEvent);
    }
}