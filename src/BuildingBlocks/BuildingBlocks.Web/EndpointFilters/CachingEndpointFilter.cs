using BuildingBlocks.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Web.EndpointFilters;

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
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        var ct = context.HttpContext.RequestAborted; //работает?
        
        var cachedResult = await _cache.GetStringAsync(request.CacheKey, ct);
        if (!string.IsNullOrEmpty(cachedResult))
        {
            _logger.LogDebug("Returning cached {TRequest} from cache. CacheKey: {CacheKey}", 
                typeof(TRequest).Name, request.CacheKey);

            var result = JsonConvert.DeserializeObject<TResponse>(cachedResult);
            
            if(result != null) return TypedResults.Ok(result);
        }
        
        var response = await next(context);

        if (response is ObjectResult endpointResponse && endpointResponse.Value is TResponse)
        {
            await _cache.SetStringAsync(request.CacheKey, JsonConvert.SerializeObject(endpointResponse.Value), ct);
        }
        
        return response;
    }
}