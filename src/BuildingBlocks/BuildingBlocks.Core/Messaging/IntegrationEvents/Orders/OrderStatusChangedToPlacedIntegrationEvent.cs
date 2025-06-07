namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderStatusChangedToPlacedIntegrationEvent(Guid OrderId, Guid CustomerId) : IntegrationEvent;