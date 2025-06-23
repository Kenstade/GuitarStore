namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderStatusChangedToAwaitingValidationIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId, 
    Guid CustomerId, 
    Guid AddressId,
    ICollection<OrderItemSummary> OrderItems) 
    : IntegrationEvent;