using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.EFCore.Interceptors;
public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if(dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        var events = dbContext.ChangeTracker
            .Entries<IAggregate>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity)
            .SelectMany(aggregate =>
            { 
                var domainEvents = aggregate.DomainEvents;

                aggregate.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccuredOn = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(domainEvent, 
                new JsonSerializerSettings 
                { 
                    TypeNameHandling = TypeNameHandling.All 
                })
            })
            .ToList();
        
        dbContext.Set<OutboxMessage>().AddRange(events);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
