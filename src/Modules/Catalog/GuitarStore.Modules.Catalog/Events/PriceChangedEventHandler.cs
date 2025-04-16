using BuildingBlocks.Core.Messaging.IntegrationEvents;
using MassTransit;

namespace GuitarStore.Modules.Catalog.Events;

public sealed class PriceChangedEventHandler : IConsumer<PriceChanged>
{
    private readonly IBus _bus;
    public PriceChangedEventHandler(IBus bus)
    {
        _bus = bus;
    }
    public async Task Consume(ConsumeContext<PriceChanged> context)
    {
        await _bus.Publish(new PriceChangedIntegrationEvent(context.Message.ProductId, context.Message.Price), 
            context.CancellationToken);
    }
}
