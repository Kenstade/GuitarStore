using BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;
using MassTransit;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Order.Data;

namespace MusicStore.Modules.Order.Events.Integration;

internal sealed class CartItemsReceivedConsumer(OrderDbContext dbContext, ILogger<CartItemsReceivedConsumer> logger) 
    : IConsumer<CartItemsReceivedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CartItemsReceivedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(CartItemsReceivedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var order = await dbContext.Orders
            .FindAsync(context.Message.OrderId);

        if (order is null) throw new NotImplementedException();

        foreach (var item in context.Message.Items)
        {
            order.AddItem(item.ProductId, item.Name, item.Image, item.Price, item.Quantity, context.Message.OrderId);
        }

        order.SetAwaitingValidationStatus(context.Message.CorrelationId);
        
        await dbContext.SaveChangesAsync();
        
        
    }
}