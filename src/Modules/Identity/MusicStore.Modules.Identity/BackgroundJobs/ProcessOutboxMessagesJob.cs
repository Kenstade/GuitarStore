using System.Data;
using BuildingBlocks.Core.Dapper;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Serialization;
using Dapper;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MusicStore.Modules.Identity.BackgroundJobs;

[DisableConcurrentExecution(10)]
internal sealed class ProcessOutboxMessagesJob(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<ProcessOutboxMessagesJob> logger, 
    IEventPublisher publisher)
{
    public async Task ProcessAsync()
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        
        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, 
                    SerializerSettings.Instance)!;

                await publisher.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{Module}] An error occured while processing message - '{MessageId}'.", 
                    Constants.ModuleName, outboxMessage.Id);
                
                exception = ex;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }
        
        await transaction.CommitAsync();

    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection,
        IDbTransaction transaction)
    {
        const string sql =
            $"""
             SELECT
             id AS {nameof(OutboxMessageResponse.Id)},
             content AS {nameof(OutboxMessageResponse.Content)}
             FROM identity.outbox_messages
             WHERE processed_on IS NULL
             ORDER BY occured_on
             LIMIT 100
             FOR UPDATE
             """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        
        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync( IDbConnection connection, IDbTransaction transaction,
        OutboxMessageResponse outboxMessage, Exception? exception)
    {
        const string sql =
            """
            UPDATE identity.outbox_messages
            SET processed_on = @ProcessedOn, error = @Error
            WHERE id = @Id;
            """;
        
        await connection.ExecuteAsync(sql, new
        {
            outboxMessage.Id,
            ProcessedOn = DateTime.UtcNow,
            Error = exception?.ToString()
        }, 
        transaction: transaction);
    }
    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
