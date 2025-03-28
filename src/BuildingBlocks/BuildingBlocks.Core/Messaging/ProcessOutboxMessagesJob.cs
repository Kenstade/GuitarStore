using BuildingBlocks.Core.Domain;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Messaging;
[DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
public sealed class ProcessOutboxMessagesJob //TODO: BackgroundService вместо hangfire?
{
    private readonly MessageDbContext _dbContext; //TODO: удалить контекст и реализовать unitofwork?
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    public ProcessOutboxMessagesJob(MessageDbContext dbContext, IPublisher publisher, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }//TODO: добавить ct, 
    public async Task ProcessAsync()
    {
        try
        {
            var messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .Take(20)
            .ToListAsync();

            foreach (var outboxMessage in messages)
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, 
                    new JsonSerializerSettings 
                    { 
                        TypeNameHandling = TypeNameHandling.All
                    });

                if (domainEvent is null) continue; //добавить лог => выяснить почему null

                await _publisher.Publish(domainEvent); //TODO: добавить в паблишер cancellationToken

                outboxMessage.ProcessedOn = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (OperationCanceledException) 
        {
            _logger.LogInformation("ProcessOutboxMessageJob cancelled"); 
        }
        catch (Exception ex) 
        { 
            _logger.LogError(ex, "An error occured in ProcessOutboxMessageJob"); 
        }
        finally 
        { 
            _logger.LogInformation("ProcessOutboxMessageJob finished..."); 
        }
    }
}
