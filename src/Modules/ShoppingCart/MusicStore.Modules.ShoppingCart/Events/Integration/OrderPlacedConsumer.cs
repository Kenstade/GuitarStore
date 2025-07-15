using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.ShoppingCart.Data;

namespace MusicStore.Modules.ShoppingCart.Events.Integration;

internal sealed class OrderPlacedConsumer(
    CartsDbContext dbContext, 
    IBus bus, 
    ILogger<OrderPlacedConsumer> logger) 
    : IConsumer<OrderPlacedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderPlacedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var integrationEvent = await dbContext.Carts
            .Where(c => c.CustomerId == context.Message.CustomerId)
            .Select(c => new CartItemsReceivedIntegrationEvent
            (
                context.Message.CorrelationId,
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