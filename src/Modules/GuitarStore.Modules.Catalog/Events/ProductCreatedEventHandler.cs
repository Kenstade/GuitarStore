using MediatR;

namespace GuitarStore.Modules.Catalog.Events;

public sealed class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        await Task.Delay(5000);
        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAaaa");

        await Task.CompletedTask;
    }
}
