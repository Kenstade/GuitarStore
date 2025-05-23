using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace BuildingBlocks.Web.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetailsOptions AddCustomProblemDetails(this ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance =
                $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

            context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
        };
        
        return options;
    }
}