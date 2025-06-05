using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.ErrorHandling;

public sealed class GlobalExceptionHandler(IProblemDetailsService pd, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (logLevel, error) = exception switch
        {
            ProblemDetailsException ex => (LogLevel.Error, ex.Error),
            
            OperationCanceledException => (LogLevel.Information, new ProblemDetails
            {
                Status = StatusCodes.Status499ClientClosedRequest,
                Title = "Request cancelled",
                Detail = exception.Message,
            }),
            
            _ => (LogLevel.Critical, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = exception.Message
            })
        };

        httpContext.Response.StatusCode = error.Status ?? StatusCodes.Status500InternalServerError;
        
        logger.Log(logLevel, exception, "Exception occurred: {Message}", exception.Message);

        return await pd.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = error
        });
    }
}

