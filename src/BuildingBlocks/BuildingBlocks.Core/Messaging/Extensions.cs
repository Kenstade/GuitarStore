using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, params Assembly[] assemblies)
    {
        //регистрация internal consumers. Сделать конфигурацию mt отельно в каждом модуле без автомат регистрации. 
        var consumers = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && 
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
            ).ToArray();
        
        services.AddMassTransit(busCfg =>
        {
            busCfg.SetKebabCaseEndpointNameFormatter();
            busCfg.AddConsumers(consumers);
            
            busCfg.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            busCfg.AddMediator(mCfg =>
            {
                mCfg.AddConsumers(consumers);
            });
        }); 

        return services;
    }
}