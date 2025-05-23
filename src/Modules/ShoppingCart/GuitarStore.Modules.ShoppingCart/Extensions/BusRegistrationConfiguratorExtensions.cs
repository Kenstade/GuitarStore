using GuitarStore.Modules.ShoppingCart.Events.Integration;
using MassTransit;

namespace GuitarStore.Modules.ShoppingCart.Extensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddOrdersModuleConsumers(this IBusRegistrationConfigurator configurator)
    {
        
        return configurator;
    }
}