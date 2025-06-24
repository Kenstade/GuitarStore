using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Orders.Data;

namespace MusicStore.Modules.Orders.Events.Domain;

internal sealed record OrderStatusChangedToAwaitingValidation(Guid CorrelationId, Guid OrderId) 
    : DomainEvent;

internal sealed class OrderStatusChangedToAwaitingValidationHandler(
    IBus bus, 
    IServiceScopeFactory scopeFactory,
    ILogger<OrderStatusChangedToAwaitingValidationHandler> logger) 
    : IEventHandler<OrderStatusChangedToAwaitingValidation>
{
    public async Task Handle(OrderStatusChangedToAwaitingValidation domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderStatusChangedToAwaitingValidation), domainEvent.Id, domainEvent.CorrelationId);
        
        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        var integrationEvent = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == domainEvent.OrderId)
            .Select(o => new OrderStatusChangedToAwaitingValidationIntegrationEvent
            (
                domainEvent.CorrelationId,
                domainEvent.OrderId,
                o.CustomerId,
                o.AddressId,
                o.Items.Select(oi => new OrderItemSummary
                (
                    oi.ProductId,
                    oi.Quantity
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (integrationEvent is null) throw new NotImplementedException();
        
        await bus.Publish(integrationEvent, cancellationToken);
    }
}