using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMassTransit(busCfg =>
        {
            busCfg.SetKebabCaseEndpointNameFormatter();
            busCfg.AddConsumers(assemblies);
            
            busCfg.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        }); 

        return services;
    }
}