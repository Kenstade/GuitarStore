using BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Orders.Data;

namespace MusicStore.Modules.Orders.Events.Integration;

internal sealed class OrderStockConfirmationFailedConsumer(
    OrdersDbContext dbContext,
    ILogger<OrderStockConfirmationFailedConsumer> logger) 
    : IConsumer<OrderStockConfirmationFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStockConfirmationFailedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderStockConfirmationFailedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);

        var order = await dbContext.Orders
            .Where(o => o.Id == context.Message.OrderId)
            .FirstOrDefaultAsync();

        if (order is null) throw new NotImplementedException();
        
        order.SetCancelledStatus();
        
        await dbContext.SaveChangesAsync();
    }
}