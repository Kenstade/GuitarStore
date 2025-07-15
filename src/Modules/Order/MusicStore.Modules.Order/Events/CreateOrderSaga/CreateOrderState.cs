using MassTransit;

namespace MusicStore.Modules.Order.Events.CreateOrderSaga;

public sealed class CreateOrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = null!;
    public int Validated { get; set; }
    
    public Guid OrderId { get; set; }
}