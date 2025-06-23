namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;

public sealed record OrderStockReservedIntegrationEvent(Guid CorrelationId) : IntegrationEvent;