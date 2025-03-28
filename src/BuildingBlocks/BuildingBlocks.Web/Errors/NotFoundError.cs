using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Errors;

public class NotFoundError : ProblemDetails
{
    protected NotFoundError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status404NotFound;
        Detail = detail;
    }
}
