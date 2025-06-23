namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record CustomerAddressAddedToOrderIntegrationEvent(Guid CorrelationId) : IntegrationEvent;