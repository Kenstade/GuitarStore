using GuitarStore.Modules.Catalog.Events.Integration;
using MassTransit;

namespace GuitarStore.Modules.Catalog.Extensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddCatalogModuleConsumers(this IBusRegistrationConfigurator configurator)
    {
        
        return configurator;
    }
}