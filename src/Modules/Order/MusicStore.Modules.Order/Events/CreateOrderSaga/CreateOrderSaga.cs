using BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using BuildingBlocks.Core.Messaging.IntegrationEvents.ShoppingCart;
using MassTransit;

namespace MusicStore.Modules.Order.Events.CreateOrderSaga;

public sealed class CreateOrderSaga : MassTransitStateMachine<CreateOrderState>
{
    public State OrderPlaced { get; init; } = null!;
    public State CartItemsReceived { get; init; } = null!;
    public State AwaitingValidation { get; init; } = null!;
    public State StockConfirmed { get; init; } = null!;
    public State AddressConfirmed { get; init; } = null!;
    public State AddressConfirmationFailed { get; init; } = null!;
    public State AddressAdded { get; init; } = null!;
    public State AwaitingPayment { get; init; } = null!;
    
    public Event<OrderPlacedIntegrationEvent> OrderPlacedEvent { get; init; } = null!;
    public Event<CartItemsReceivedIntegrationEvent> CartItemsReceivedEvent { get; init; } = null!;
    public Event<OrderStatusChangedToAwaitingValidationIntegrationEvent> OrderStatusChangedToAwaitingValidationEvent { get; init; } = null!;
    public Event<OrderStockReservedIntegrationEvent> OrderStockConfirmedEvent { get; init; } = null!;
    public Event<CustomerAddressConfirmedIntegrationEvent> CustomerAddressConfirmedEvent { get; init; } = null!;
    public Event<CustomerAddressConfirmationFailedIntegrationEvent> CustomerAddressConfirmationFailedEvent { get; init; } = null!;
    public Event<CustomerAddressAddedToOrderIntegrationEvent> CustomerAddressAddedToOrderEvent { get; init; } = null!;
    public Event OrderValidatedEvent { get; init; } = null!;

    public CreateOrderSaga()
    {
        Event(() => OrderPlacedEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CartItemsReceivedEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => OrderStatusChangedToAwaitingValidationEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => OrderStockConfirmedEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CustomerAddressConfirmedEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CustomerAddressConfirmationFailedEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CustomerAddressAddedToOrderEvent, c => c.CorrelateById(m => m.Message.CorrelationId));
        
        InstanceState(s => s.CurrentState);
        
        Initially(
            When(OrderPlacedEvent)
                .Then(context => context.Saga.OrderId = context.Message.OrderId)
                .TransitionTo(OrderPlaced));
        
        During(OrderPlaced,
            When(CartItemsReceivedEvent)
                .TransitionTo(CartItemsReceived));
        
        During(CartItemsReceived,
            When(OrderStatusChangedToAwaitingValidationEvent)
                .TransitionTo(AwaitingValidation));
        
        During(AwaitingValidation,
            When(OrderStockConfirmedEvent)
                .TransitionTo(StockConfirmed),
            When(CustomerAddressConfirmedEvent)
                .TransitionTo(AddressConfirmed),
            When(CustomerAddressConfirmationFailedEvent)
                .TransitionTo(AddressConfirmationFailed)
                .Finalize());
        
        During(StockConfirmed,
            When(CustomerAddressConfirmedEvent)
                .TransitionTo(AddressConfirmed));
        
        During(AddressConfirmed,
            When(CustomerAddressAddedToOrderEvent)
                .TransitionTo(AddressAdded));
        
        During(AddressAdded,
            When(OrderStockConfirmedEvent)
                .TransitionTo(StockConfirmed));

        CompositeEvent(() => OrderValidatedEvent, state => state.Validated,
            OrderStockConfirmedEvent, CustomerAddressAddedToOrderEvent);
        
        DuringAny(
            When(OrderValidatedEvent)                                     
                .Publish(context => new OrderValidatedIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                .TransitionTo(AwaitingPayment)
                .Finalize());
    }
}