using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Caching;

public sealed class CachingEndpointFilter<TRequest, TResponse> : IEndpointFilter where TRequest : ICacheRequest
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingEndpointFilter<TRequest, TResponse>> _logger;
    public CachingEndpointFilter(IDistributedCache cache, ILogger<CachingEndpointFilter<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var ct = context.HttpContext.RequestAborted;
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if(request == null) return await next(context);
        
        var cachedResult = await _cache.GetStringAsync(request.CacheKey, ct);
        if (!string.IsNullOrEmpty(cachedResult))
        {
            var result = JsonConvert.DeserializeObject<TResponse>(cachedResult);
            
            _logger.LogDebug("Return cached {TRequest} from cache. CacheKey: {CacheKey}", 
                typeof(TRequest).Name, request.CacheKey);
            
            return TypedResults.Ok(result);
        }
        
        var response = await next(context);

        if (response is ObjectResult endpointResponse && endpointResponse.Value is TResponse)
        {
            await _cache.SetStringAsync(request.CacheKey, JsonConvert.SerializeObject(endpointResponse.Value), ct);
        }
        
        return response;
    }
}