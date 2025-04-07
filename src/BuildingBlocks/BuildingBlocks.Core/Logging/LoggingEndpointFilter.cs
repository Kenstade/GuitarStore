using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Logging;

public class LoggingEndpointFilter<TRequest> : IEndpointFilter
{
    private readonly ILogger<LoggingEndpointFilter<TRequest>> _logger;

    public LoggingEndpointFilter(ILogger<LoggingEndpointFilter<TRequest>> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        const string prefix = nameof(LoggingEndpointFilter<TRequest>);
        _logger.LogInformation("[{prefix}] Processing {request}", prefix ,typeof(TRequest).Name);
        
        var result = await next(context);
        if (context.HttpContext.Response.StatusCode == 200)
        {
            _logger.LogInformation("[{prefix}] Completed {request}", prefix ,typeof(TRequest).Name);
        }
        else
        {
            _logger.LogError("[{prefix}] Completed {request} with {Error}", 
                prefix, typeof(TRequest).Name, context.HttpContext.Response.StatusCode);
        }
        
        return result;
    }
}