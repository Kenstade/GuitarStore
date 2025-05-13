using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Extensions;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        
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
        
        return services;
    }
}
