using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MusicStore.Modules.Orders.Events.Domain;

internal sealed class OrderPaymentRefundWhenOrderCancelledHandler(
    IBus bus,
    ILogger<OrderPaymentRefundWhenOrderCancelledHandler> logger
    ) : IEventHandler<OrderCancelled>
{
    public async Task Handle(OrderCancelled domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderCancelled), domainEvent.Id, domainEvent.CorrelationId);

        await bus.Publish(new OrderPaymentRefundedIntegrationEvent(domainEvent.CorrelationId), cancellationToken);
    }
}