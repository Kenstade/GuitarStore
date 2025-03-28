using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        if (postgresOptions == null) throw new ArgumentNullException(nameof(postgresOptions));

        services.AddHangfire(config => 
        {
            if (!postgresOptions.UseInMemory)
            {
                config.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(postgresOptions.ConnectionString));
            }
            else
            {
                config.UseMemoryStorage();
            }
        });

        services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

        services.AddScoped<ProcessOutboxMessagesJob>();

        return services;
    }

    public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app, IConfiguration configuration)
    {
        var hangfireOptions = configuration.GetOptions<HangfireOptions>(nameof(HangfireOptions));
        if (hangfireOptions == null) throw new ArgumentNullException(nameof(hangfireOptions));

        app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ProcessOutboxMessagesJob>("outbox-processor", job => 
            job.ProcessAsync(), hangfireOptions.OutboxSchedule);

        return app;
    }
}
