using MassTransit;

namespace GuitarStore.Modules.Orders.Events.CancelOrderSaga;

public sealed class CancelOrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = null!;
    
    public int CancellationCompleted { get; set; }
}