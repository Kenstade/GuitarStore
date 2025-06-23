using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using GuitarStore.Modules.Orders.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Orders.Events.Integration;

internal sealed class OrderValidatedConsumer(
    OrdersDbContext dbContext,
    ILogger<OrderValidatedConsumer> logger) 
    : IConsumer<OrderValidatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderValidatedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderValidatedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var order = await dbContext.Orders
            .Where(o => o.Id == context.Message.OrderId)
            .FirstOrDefaultAsync();

        if (order is null) throw new NotImplementedException();
        
        order.SetAwaitingPaymentStatus();
        
        await dbContext.SaveChangesAsync();
    }
}