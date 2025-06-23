namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

public sealed record OrderItemSummary(Guid ProductId, int Quantity);