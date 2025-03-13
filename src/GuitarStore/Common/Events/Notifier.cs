namespace GuitarStore.Common.Events;

internal sealed class Notifier(IServiceProvider sp, ILogger<Notifier> logger) : INotifier
{
    private readonly IServiceProvider _sp = sp;
    private readonly ILogger<Notifier> _logger = logger;

    public Task Send<TEvent>(TEvent message) where TEvent : IEvent
    {
        using var scope = _sp.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            _logger.LogInformation("Handling {event}", typeof(TEvent).Name);

           handler.Handle(message);

            _logger.LogInformation("{event} completed", typeof(TEvent).Name);
        }

        return Task.CompletedTask;
       
    }
}
