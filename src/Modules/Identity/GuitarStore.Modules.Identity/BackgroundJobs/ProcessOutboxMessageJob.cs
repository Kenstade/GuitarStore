using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Identity.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GuitarStore.Modules.Identity.BackgroundJobs;

[DisableConcurrentExecution(60)]
internal sealed class ProcessOutboxMessageJob
{
    private readonly IdentityContext _dbContext;
    private readonly ILogger<ProcessOutboxMessageJob> _logger;
    private readonly IEventPublisher _publisher;
    public ProcessOutboxMessageJob(IdentityContext dbContext, ILogger<ProcessOutboxMessageJob> logger, IEventPublisher publisher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _publisher = publisher;
    }
    
    public async Task ProcessAsync()
    {
        var messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .Take(20)
            .ToListAsync();

        foreach (var outboxMessage in messages)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                        // MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
                    });

                if (domainEvent is null)
                {
                    _logger.LogWarning("Failed to deserialize outbox message. ID: {MessageId}", outboxMessage.Id);
                    continue;
                }

                await _publisher.Publish(domainEvent);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Module} - An error occured while processing message. ID: {MessageId}", 
                    IdentityModule.ModuleName, outboxMessage.Id);
                
                outboxMessage.Error = ex.ToString();
            }
            
            outboxMessage.ProcessedOn = DateTime.UtcNow;
            
            _dbContext.Update(outboxMessage);
            await _dbContext.SaveChangesAsync();
        }
    }
}
