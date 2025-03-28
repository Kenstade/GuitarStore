using Microsoft.Extensions.Caching.Memory;

namespace GuitarStore.Common.Caching;

internal sealed class MemoryCacheProvider(IMemoryCache cache, ILogger<MemoryCacheProvider> logger) : ICacheProvider
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<MemoryCacheProvider> _logger = logger;
    private readonly SemaphoreSlim Semaphore = new(1,1);

    private readonly TimeSpan defaultAbsoluteExpiration = TimeSpan.FromMinutes(30);
    private readonly TimeSpan defaultSlidingExpiration = TimeSpan.FromMinutes(5);

    public async Task<TResponse> GetOrCreateAsync<TResponse>(ICacheRequest request, Func<Task<TResponse>> factory)
    {
        if (!_cache.TryGetValue(request.CacheKey, out TResponse? response))
        {
            try
            {
                await Semaphore.WaitAsync();

                response = await factory();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(defaultAbsoluteExpiration)
                   .SetSlidingExpiration(defaultSlidingExpiration);

                _logger.LogInformation("Caching response with cache key: {CacheKey}", request.CacheKey);

                _cache.Set(request.CacheKey, response, cacheEntryOptions);
            }
            finally { Semaphore.Release(); }
        }
        else _logger.LogInformation("Response retrieved from cache. Cache key: {CacheKey}", request.CacheKey);

        return response;
    }
}
