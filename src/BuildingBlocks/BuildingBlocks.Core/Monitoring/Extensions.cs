using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Monitoring;

public static class Extensions
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        var redisOptions = configuration.GetOptions<RedisOptions>(nameof(RedisOptions));
        
        services.AddHealthChecks()
            .AddNpgSql(postgresOptions.ConnectionString)
            .AddRedis(redisOptions.ConnectionString);
        
        return services;
    }
}