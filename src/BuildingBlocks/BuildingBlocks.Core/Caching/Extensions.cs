using BuildingBlocks.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Caching;

public static class Extensions
{
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetOptions<RedisOptions>(nameof(RedisOptions));

        if (redisOptions.UseInMemory)
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisOptions.ConnectionString;
            });
        }
        
        return services;
    }
}