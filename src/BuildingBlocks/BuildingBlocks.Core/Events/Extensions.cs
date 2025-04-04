using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Events;

public static class Extensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services, Assembly assembly)
    {
        var eventPublisher = new EventPublisher();
        
        var handlerTypes = assembly.GetExportedTypes()
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition()
                == typeof(IEventHandler<>)));
        
        foreach (var handlerType in handlerTypes)
        {
            // var handlerInstance = Activator.CreateInstance(handlerType);
            var notificationType = handlerType.GetInterfaces().First().GetGenericArguments().First();
            var registerMethod = typeof(EventPublisher).GetMethod("Subscribe").MakeGenericMethod(notificationType);
            registerMethod.Invoke(eventPublisher, new object[] { Activator.CreateInstance(handlerType) });
            
        }

        services.AddSingleton<IEventPublisher>(eventPublisher);
        return services;
    }
}