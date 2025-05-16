using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Caching;

public sealed class CachingEndpointFilter<TRequest, TResponse> : IEndpointFilter
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingEndpointFilter<TRequest, TResponse>> _logger;
    public CachingEndpointFilter(ICacheService cache, ILogger<CachingEndpointFilter<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        const string prefix = nameof(CachingEndpointFilter<TRequest, TResponse>);
        
        var ct = context.HttpContext.RequestAborted;

        var cachedResult = await _cache.GetAsync<TResponse>(typeof(TRequest).Name, ct);
        if (cachedResult != null)
        {
            _logger.LogInformation("[{Prefix}] Return response from cache. CacheKey: {CacheKey}", 
                prefix, typeof(TRequest).Name);
            
            return TypedResults.Ok(cachedResult);
        }
        
        var response = await next(context);

        if (response is Ok<TResponse> ok)
        {
            await _cache.SetAsync(typeof(TRequest).Name, ok.Value, ct);
        }
        
        return response;
    }
}