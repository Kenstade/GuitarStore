using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Caching;

public sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var result = await cache.GetStringAsync(key, ct);
        
        return result is null ? default : JsonConvert.DeserializeObject<T>(result);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken ct = default, TimeSpan? expiration = null)
    {
        await cache.SetStringAsync(key, JsonConvert.SerializeObject(value),CacheOptions.Create(expiration), ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await cache.RemoveAsync(key, ct);
    }
}