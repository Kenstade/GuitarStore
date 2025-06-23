using BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using GuitarStore.Modules.Customers.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Customers.Events.Integration;

internal sealed class OrderStatusChangedToAwaitingValidationConsumer(
    CustomersDbContext dbContext,
    IBus bus,
    ILogger<OrderStatusChangedToAwaitingValidationConsumer> logger) 
    : IConsumer<OrderStatusChangedToAwaitingValidationIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStatusChangedToAwaitingValidationIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderStatusChangedToAwaitingValidationIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var address = await dbContext.Addresses
            .Where(a => a.CustomerId == context.Message.CustomerId && a.Id == context.Message.AddressId)
            .Select(a => new { a.City, a.Street, a.BuildingNumber, a.Apartment })
            .FirstOrDefaultAsync();

        if (address is not null)
        {
            logger.LogInformation("Address confirmed for Order {OrderId}.", context.Message.OrderId);
            
            await bus.Publish(new CustomerAddressConfirmedIntegrationEvent(
                context.Message.CorrelationId, 
                context.Message.OrderId,
                address.City, 
                address.Street, 
                address.BuildingNumber,
                address.Apartment));
        }
        else
        {
            logger.LogWarning("Address confirmation failed for Order {OrderId}.", context.Message.OrderId);
            
            await bus.Publish(new CustomerAddressConfirmationFailedIntegrationEvent(
                context.Message.CorrelationId,
                context.Message.OrderId));
        }
    }
}