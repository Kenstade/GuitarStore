using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;

namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;

public sealed record OrderStockConfirmationFailedIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId, 
    ICollection<OrderItemSummary> Items) 
    : IntegrationEvent;