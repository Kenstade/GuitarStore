using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Orders.Events.Domain;

internal sealed record OrderPlaced(Guid OrderId, Guid CustomerId) : DomainEvent;

internal sealed class OrderPlacedHandler(
    IBus bus,
    ILogger<OrderPlacedHandler> logger) 
    : IEventHandler<OrderPlaced>
{
    public async Task Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling '{DomainEvent}' - '{DomainEventId}'.",
            nameof(OrderPlaced), domainEvent.Id);

        await bus.Publish(
            new OrderPlacedIntegrationEvent(domainEvent.OrderId, domainEvent.CustomerId), 
                cancellationToken);
    }
}