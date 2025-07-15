using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Order.Data;

namespace MusicStore.Modules.Order.Events.Domain;

internal sealed record OrderCancelled(Guid CorrelationId, Guid OrderId) : DomainEvent;

internal sealed class OrderCancelledHandler(
    IServiceScopeFactory serviceScopeFactory,
    IBus bus,
    ILogger<OrderCancelledHandler> logger) 
    : IEventHandler<OrderCancelled>
{
    public async Task Handle(OrderCancelled domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderCancelled), domainEvent.Id, domainEvent.CorrelationId);
        
        using var scope = serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        var orderItems = await dbContext.OrderItems
            .AsNoTracking()
            .Where(o => o.OrderId == domainEvent.OrderId)
            .Select(o => new OrderItemSummary(o.ProductId, o.Quantity))
            .ToListAsync(cancellationToken);
        
        await bus.Publish(new OrderCancelledIntegrationEvent(domainEvent.CorrelationId, domainEvent.OrderId, orderItems), 
            cancellationToken);
    }
}