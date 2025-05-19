using BuildingBlocks.Core.Exceptions.Types;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Exceptions;

public sealed class GlobalExceptionHandler(IProblemDetailsService pd, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, logLevel, problemDetailsSummary) = exception switch
        {
            OperationCanceledException => (
                StatusCodes.Status499ClientClosedRequest,
                LogLevel.Information,
                new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = "Request canceled",
                    Detail = exception.Message,
                }),
            
            ApplicationException => (
                StatusCodes.Status400BadRequest,
                LogLevel.Error,
                CreateDefaultProblemDetails(exception)),
            
            DomainException => (
                StatusCodes.Status409Conflict,
                LogLevel.Error,
                CreateDefaultProblemDetails(exception)),
            
            _ => (
                StatusCodes.Status500InternalServerError,
                LogLevel.Error,
                CreateDefaultProblemDetails(exception))
        };
        
        httpContext.Response.StatusCode = statusCode;
        logger.Log(logLevel, exception, "Exception occurred: {Message}", exception.Message);

        return await pd.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = problemDetailsSummary.Type,
                Title = problemDetailsSummary.Title,
                Detail = problemDetailsSummary.Detail,
                Status = statusCode
            }
        });
    }

    private static ProblemDetails CreateDefaultProblemDetails(Exception ex) => new()
    {
        Type = ex.GetType().Name,
        Title = "An error occurred",
        Detail = ex.Message
    };
}

