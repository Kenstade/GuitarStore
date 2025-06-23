using BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;
using GuitarStore.Modules.Orders.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Orders.Events.Integration;

internal sealed class CustomerAddressConfirmedConsumer(
    OrdersDbContext dbContext,
    ILogger<CustomerAddressConfirmedConsumer> logger) 
    : IConsumer<CustomerAddressConfirmedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CustomerAddressConfirmedIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(CustomerAddressConfirmedIntegrationEvent), context.Message.Id, context.Message.CorrelationId);

        var order = await dbContext.Orders
            .Where(o => o.Id == context.Message.OrderId)
            .FirstOrDefaultAsync();

        if (order is null) throw new NotImplementedException();
        
        order.AddAddress(
            context.Message.CorrelationId,
            context.Message.City, 
            context.Message.Street, 
            context.Message.BuildingNumber, 
            context.Message.Apartment);
        
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Customer address added to Order '{OrderId}'.", context.Message.OrderId);
    }
}