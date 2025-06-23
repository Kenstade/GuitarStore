using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Orders.Events.Domain;

internal sealed record CustomerAddressAddedToOrder(Guid CorrelationId) : DomainEvent;

internal sealed class CustomerAddressAddedToOrderHandler(
    IBus bus,
    ILogger<CustomerAddressAddedToOrderHandler> logger)
    : IEventHandler<CustomerAddressAddedToOrder> 
{
    public async Task Handle(CustomerAddressAddedToOrder domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(CustomerAddressAddedToOrder), domainEvent.Id, domainEvent.CorrelationId);

        await bus.Publish(new CustomerAddressAddedToOrderIntegrationEvent(domainEvent.CorrelationId), cancellationToken);
    }
}