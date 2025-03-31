using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Caching;

public sealed class MemoryCacheProvider(IMemoryCache cache, ILogger<MemoryCacheProvider> logger) : ICacheProvider
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<MemoryCacheProvider> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new(1,1);

    private readonly TimeSpan _defaultAbsoluteExpiration = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _defaultSlidingExpiration = TimeSpan.FromMinutes(5);

    public async Task<TResponse> GetOrCreateAsync<TResponse>(ICacheRequest request, Func<Task<TResponse>> factory)
    {//TODO: add ct
        if (!_cache.TryGetValue(request.CacheKey, out TResponse? response))
        {
            try
            {
                await _semaphore.WaitAsync();

                response = await factory();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(_defaultAbsoluteExpiration)
                   .SetSlidingExpiration(_defaultSlidingExpiration);

                _logger.LogInformation("Caching response with cache key: {CacheKey}", request.CacheKey);

                _cache.Set(request.CacheKey, response, cacheEntryOptions);
            }
            finally { _semaphore.Release(); }
        }
        else _logger.LogInformation("Response retrieved from cache. Cache key: {CacheKey}", request.CacheKey);

        return response;
    }
}
