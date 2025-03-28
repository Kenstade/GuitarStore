using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Errors;

public class UnauthorizedError : ProblemDetails
{
    protected UnauthorizedError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status401Unauthorized;
        Detail = detail;
    }
}