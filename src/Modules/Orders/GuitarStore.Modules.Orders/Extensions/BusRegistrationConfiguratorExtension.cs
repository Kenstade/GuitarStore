using GuitarStore.Modules.Orders.Events.Integration;
using MassTransit;

namespace GuitarStore.Modules.Orders.Extensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddIdentityModuleConsumers(this IBusRegistrationConfigurator configurator)
    {
            
        return configurator;
    }
}