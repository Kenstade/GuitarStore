namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderValidatedIntegrationEvent(Guid CorrelationId, Guid OrderId) : IntegrationEvent;