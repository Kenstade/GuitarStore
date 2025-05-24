using GuitarStore.Modules.Customers.Events.Integration;
using MassTransit;

namespace GuitarStore.Modules.Customers.Extensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddCustomersModuleConsumers(this IBusRegistrationConfigurator configurator)
    {
        
        return configurator;
    }
}