namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderCancelledIntegrationEvent(
    Guid CorrelationId, 
    Guid OrderId, 
    ICollection<OrderItemSummary> OrderItems) 
    : IntegrationEvent;