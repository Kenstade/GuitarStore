using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Orders.Events.Domain;

internal sealed record OrderStatusChangedToPlaced(Guid OrderId, Guid CustomerId) : DomainEvent;

internal sealed class OrderStatusChangedToPlacedHandler(IBus bus, ILogger<OrderStatusChangedToPlacedHandler> logger ) 
    : IEventHandler<OrderStatusChangedToPlaced>
{
    public async Task Handle(OrderStatusChangedToPlaced domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling '{DomainEvent}' - '{DomainEventId}'.",
            nameof(OrderStatusChangedToPlaced), domainEvent.Id);
        
        await bus.Publish(new OrderStatusChangedToPlacedIntegrationEvent(domainEvent.OrderId, domainEvent.CustomerId), 
            cancellationToken);
    }
}