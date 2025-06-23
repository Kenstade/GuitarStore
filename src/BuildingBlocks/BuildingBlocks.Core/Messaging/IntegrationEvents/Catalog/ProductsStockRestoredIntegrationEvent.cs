namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;

public sealed record ProductsStockRestoredIntegrationEvent(Guid CorrelationId) : IntegrationEvent;