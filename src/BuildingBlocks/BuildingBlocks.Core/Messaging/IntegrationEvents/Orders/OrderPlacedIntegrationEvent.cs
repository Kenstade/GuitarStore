namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderPlacedIntegrationEvent(
    Guid CorrelationId, 
    Guid OrderId, 
    Guid CustomerId) : IntegrationEvent;