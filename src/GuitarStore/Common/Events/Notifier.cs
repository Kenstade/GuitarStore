namespace GuitarStore.Common.Events;

internal sealed class Notifier(IServiceProvider sp) : INotifier
{
    private readonly IServiceProvider _sp = sp;

    public Task Send<T>(T notification) where T : INotification
    {
        using var scope = _sp.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<INotificationHandler<T>>();

        foreach (var handler in handlers)
        {
           handler.Handle(notification);
        }

        return Task.CompletedTask;
       
    }
}
