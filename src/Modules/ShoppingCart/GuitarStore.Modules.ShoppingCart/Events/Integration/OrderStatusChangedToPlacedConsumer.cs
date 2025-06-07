using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;
using GuitarStore.Modules.ShoppingCart.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.ShoppingCart.Events.Integration;

internal sealed class OrderStatusChangedToPlacedConsumer(
    CartDbContext dbContext, 
    IBus bus, 
    ILogger<OrderStatusChangedToPlacedConsumer> logger) 
    : IConsumer<OrderStatusChangedToPlacedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStatusChangedToPlacedIntegrationEvent> context)
    {
        logger.LogInformation("Handling '{IntegrationEvent}' - '{IntegrationEventId}'.", 
            nameof(OrderStatusChangedToPlacedIntegrationEvent), context.Message.Id); 
        
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