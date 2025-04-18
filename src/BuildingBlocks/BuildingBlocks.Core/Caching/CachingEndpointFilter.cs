using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Caching;

public sealed class CachingEndpointFilter<TRequest, TResponse> : IEndpointFilter where TRequest : ICacheRequest
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
        var ct = context.HttpContext.RequestAborted;
        
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if(request == null) return await next(context);

        var cachedResult = await _cache.GetAsync<TResponse>(request.CacheKey, ct);
        if (cachedResult != null)
        {
            _logger.LogInformation("Return cached '{TRequest}' from cache. CacheKey: {CacheKey}", 
                typeof(TRequest).Name, request.CacheKey);
            
            return TypedResults.Ok(cachedResult);
        }
        
        var response = await next(context);

        if (response is Ok<TResponse> ok)
        {
            await _cache.SetAsync(request.CacheKey, ok.Value);
        }
        
        return response;
    }
}