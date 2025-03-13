using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Common.Web.Errors;

public class UnauthorizedError : ProblemDetails
{
    protected UnauthorizedError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status401Unauthorized;
        Detail = detail;
    }
}