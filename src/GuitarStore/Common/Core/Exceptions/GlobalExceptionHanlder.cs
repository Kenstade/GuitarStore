using GuitarStore.Common.Core.Exceptions.Types;
using GuitarStore.Identity.Features;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Common.Core.Exceptions;

internal sealed class GlobalExceptionHanlder(IProblemDetailsService pd, ILogger<GlobalExceptionHanlder> logger)
    : IExceptionHandler
{
    private readonly IProblemDetailsService _pd = pd;
    private readonly ILogger<GlobalExceptionHanlder> _logger = logger;
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            DomainException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return await _pd.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occured",
                Detail = exception.Message,
            }
        });
    }
}
