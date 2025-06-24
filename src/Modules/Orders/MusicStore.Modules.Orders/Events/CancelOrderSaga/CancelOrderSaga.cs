using BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;

namespace MusicStore.Modules.Orders.Events.CancelOrderSaga;

public sealed class CancelOrderSaga : MassTransitStateMachine<CancelOrderState>
{
    public State CancellationStarted { get; init; } = null!;
    public State PaymentRefunded { get; init; } = null!;
    public State StockRestored { get; init; } = null!;
    
    public Event<OrderCancelledIntegrationEvent> OrderCancelled { get; init; } = null!;
    public Event<OrderPaymentRefundedIntegrationEvent> OrderPaymentRefunded { get; init; } = null!;
    public Event<ProductsStockRestoredIntegrationEvent> ProductsStockRestored { get; init; } = null!;
    public Event OrderCancellationCompleted { get; init; } = null!;

    public CancelOrderSaga()
    {
        Event(() => OrderCancelled, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => OrderPaymentRefunded, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ProductsStockRestored, c => c.CorrelateById(m => m.Message.CorrelationId));
        
        InstanceState(s => s.CurrentState);
        
        Initially(
            When(OrderCancelled)
                .TransitionTo(CancellationStarted));
        
        During(CancellationStarted, 
            When(OrderPaymentRefunded)
                .TransitionTo(PaymentRefunded),
            When(ProductsStockRestored)
                .TransitionTo(StockRestored));
        
        During(PaymentRefunded,
            When(ProductsStockRestored)
                .TransitionTo(StockRestored));
        
        During(StockRestored,
            When(OrderPaymentRefunded)
                .TransitionTo(PaymentRefunded));
        
        CompositeEvent(() => OrderCancellationCompleted, state => state.CancellationCompleted,
            OrderPaymentRefunded, ProductsStockRestored);
        
        DuringAny(
            When(OrderCancellationCompleted)
                .Finalize());
    }
}