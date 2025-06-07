namespace BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;

public sealed record CartItemsReceivedIntegrationEvent(Guid OrderId, List<CartItemSummary> Items) 
    : IntegrationEvent;
