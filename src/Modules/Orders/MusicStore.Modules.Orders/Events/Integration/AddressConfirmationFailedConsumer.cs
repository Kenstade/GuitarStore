using BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Orders.Data;

namespace MusicStore.Modules.Orders.Events.Integration;

internal sealed class AddressConfirmationFailedConsumer(
    OrdersDbContext dbContext,
    ILogger<AddressConfirmationFailedConsumer> logger)
    : IConsumer<CustomerAddressConfirmationFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CustomerAddressConfirmationFailedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(CustomerAddressConfirmationFailedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);

        var order = await dbContext.Orders
            .Where(o => o.Id == context.Message.OrderId)
            .FirstOrDefaultAsync();

        if (order is null) throw new NotImplementedException();
        
        order.SetCancelledStatus();
        
        await dbContext.SaveChangesAsync(); 
    }
}