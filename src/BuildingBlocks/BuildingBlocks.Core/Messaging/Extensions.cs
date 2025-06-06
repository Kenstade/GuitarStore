using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, 
        Action<IRegistrationConfigurator> configureConsumers)
    {
        services.AddMassTransit(busCfg =>
        {
            busCfg.SetKebabCaseEndpointNameFormatter();

            configureConsumers(busCfg);
            
            busCfg.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        }); 

        return services;
    }
}