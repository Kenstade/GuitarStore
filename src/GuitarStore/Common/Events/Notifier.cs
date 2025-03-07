namespace GuitarStore.Common.Events;

internal sealed class Notifier(IServiceProvider sp, ILogger<Notifier> logger) : INotifier
{
    private readonly IServiceProvider _sp = sp;
    private readonly ILogger<Notifier> _logger = logger;

    public Task Send<TRequest>(TRequest notification) where TRequest : INotification
    {
        using var scope = _sp.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<INotificationHandler<TRequest>>();

        foreach (var handler in handlers)
        {
            _logger.LogInformation("Handling {event}", typeof(TRequest).Name);

           handler.Handle(notification);

            _logger.LogInformation("{event} completed", typeof(TRequest).Name);
        }

        return Task.CompletedTask;
       
    }
}
