namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderPaymentRefundedIntegrationEvent(Guid CorrelationId) : IntegrationEvent;