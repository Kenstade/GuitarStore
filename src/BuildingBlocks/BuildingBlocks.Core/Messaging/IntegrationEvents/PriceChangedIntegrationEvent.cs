namespace BuildingBlocks.Core.Messaging.IntegrationEvents;

public record PriceChangedIntegrationEvent(Guid ProductId, decimal Price);