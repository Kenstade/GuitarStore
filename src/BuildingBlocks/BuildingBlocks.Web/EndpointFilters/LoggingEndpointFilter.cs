using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.EndpointFilters;

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
        _logger.LogInformation("[{prefix}] Handling {request}", prefix ,typeof(TRequest).Name);
        
        var result = await next(context);
        
        _logger.LogInformation("[{prefix}] Finished handling {request}", prefix ,typeof(TRequest).Name);
        
        return result;
    }
}