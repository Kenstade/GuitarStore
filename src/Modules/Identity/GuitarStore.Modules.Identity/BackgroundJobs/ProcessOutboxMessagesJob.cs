using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.Outbox;
using BuildingBlocks.Core.Serialization;
using GuitarStore.Modules.Identity.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GuitarStore.Modules.Identity.BackgroundJobs;

[DisableConcurrentExecution(10)]
internal sealed class ProcessOutboxMessagesJob(
    IdentityContext dbContext, 
    ILogger<ProcessOutboxMessagesJob> logger, 
    IEventPublisher publisher)
{
    public async Task ProcessAsync()
    {
        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccuredOn)
            .Take(100)
            .ToListAsync();

        foreach (var outboxMessage in messages)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content,
                    SerializerSettings.Instance);

                if (domainEvent is null)
                {
                    logger.LogWarning("[{Module}] Failed to deserialize outbox message - '{MessageId}'.", 
                        IdentityModule.ModuleName, outboxMessage.Id);
                    continue;
                }

                await publisher.Publish(domainEvent);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{Module}] An error occured while processing message - '{MessageId}'.", 
                    IdentityModule.ModuleName, outboxMessage.Id);
                
                outboxMessage.Error = ex.ToString();
            }
            outboxMessage.ProcessedOn = DateTime.UtcNow;
        }
        await dbContext.SaveChangesAsync();
    }
}
