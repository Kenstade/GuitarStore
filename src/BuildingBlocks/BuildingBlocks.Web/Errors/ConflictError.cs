using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Errors;

public class ConflictError : ProblemDetails
{
    protected ConflictError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status409Conflict;
        Detail = detail;
    }
}