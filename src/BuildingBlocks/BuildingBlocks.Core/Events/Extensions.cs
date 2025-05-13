using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Events;

public static class Extensions
{
    public static IServiceCollection AddEventPublisher(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly
            .GetTypes()
            .Where(x => x.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)) && !x.IsAbstract && !x.IsInterface);
        
        foreach(var type in handlerTypes)
        {
            services.AddTransient(type.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IEventHandler<>)), type);
        }
        
        services.AddSingleton<IEventPublisher, EventPublisher>();
        
        return services;
    }
}