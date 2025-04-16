using BuildingBlocks.Core.Messaging.IntegrationEvents;
using GuitarStore.Modules.ShoppingCart.Data;
using MassTransit;

namespace GuitarStore.Modules.ShoppingCart.Events.Integration;

public sealed class PriceChangedConsumer : IConsumer<PriceChangedIntegrationEvent>
{
    // private readonly CartDbContext _dbContext;
    // public PriceChangedConsumer(CartDbContext dbContext)
    // {
    //     _dbContext = dbContext;
    // }
    public Task Consume(ConsumeContext<PriceChangedIntegrationEvent> context)
    {
        Console.WriteLine($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");
        return Task.CompletedTask;
    }
}