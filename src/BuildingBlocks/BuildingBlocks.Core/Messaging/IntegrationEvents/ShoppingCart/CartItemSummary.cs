namespace BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;


public sealed record CartItemSummary(Guid ProductId, string Name, string? Image, int Quantity, decimal Price);