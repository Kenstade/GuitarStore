using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Logging;

public sealed class LoggingEndpointFilter<TRequest>(ILogger<LoggingEndpointFilter<TRequest>> logger) 
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        const string prefix = nameof(LoggingEndpointFilter<TRequest>);
        logger.LogInformation("[{Prefix}] Processing {Request} request.", prefix ,typeof(TRequest).Name);
        
        var result = await next(context);
        if (context.HttpContext.Response.StatusCode == 200)
        {
            logger.LogInformation("[{Prefix}] Completed {Request}", prefix, typeof(TRequest).Name);
        }
        else
        {
            logger.LogError("[{Prefix}] Completed {Request} with {Error}", 
                prefix, typeof(TRequest).Name, context.HttpContext.Response.StatusCode);
        }
        
        return result;
    }
}