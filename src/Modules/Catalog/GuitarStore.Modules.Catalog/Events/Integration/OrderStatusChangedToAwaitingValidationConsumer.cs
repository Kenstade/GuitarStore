using BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using GuitarStore.Modules.Catalog.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Catalog.Events.Integration;

internal sealed class OrderStatusChangedToAwaitingValidationConsumer(
    CatalogDbContext dbContext, 
    IBus bus,
    ILogger<OrderStatusChangedToAwaitingValidationConsumer> logger) 
    : IConsumer<OrderStatusChangedToAwaitingValidationIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStatusChangedToAwaitingValidationIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderStatusChangedToAwaitingValidationIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var productIds = context.Message.OrderItems.Select(o => o.ProductId).ToList();
        var products = await dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        
        var unconfirmedOrderStockItems = new List<OrderItemSummary>();

        foreach (var item in context.Message.OrderItems)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            
            if (product is null)
            {
                logger.LogError("Product '{Id}' not found.", item.ProductId);
                
                unconfirmedOrderStockItems.Add(new OrderItemSummary(item.ProductId, item.Quantity));

                continue;
            }
            if (!product.HasAvailableStock(item.Quantity))
            {
                logger.LogError("Requested quantity({Quantity}) for product '{Id}' exceeds available stock({Stock}).", 
                    item.Quantity, item.ProductId, product.AvailableStock);
                
                unconfirmedOrderStockItems.Add(new OrderItemSummary(item.ProductId, item.Quantity));

                continue;
            }

            product.ReserveStock(item.Quantity);
        }
        
        if (!unconfirmedOrderStockItems.Any())
        {
            await dbContext.SaveChangesAsync();
            await bus.Publish(new OrderStockReservedIntegrationEvent(context.Message.CorrelationId));
        }
        else
        {
            await bus.Publish(
                new OrderStockConfirmationFailedIntegrationEvent(
                    context.Message.CorrelationId, 
                    context.Message.OrderId, 
                    unconfirmedOrderStockItems));
        }
    }
}