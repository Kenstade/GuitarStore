namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderPlacedIntegrationEvent(Guid OrderId, Guid CustomerId) : IntegrationEvent;