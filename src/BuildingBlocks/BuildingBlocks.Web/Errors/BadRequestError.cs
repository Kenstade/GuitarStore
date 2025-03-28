using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Errors;

public class BadRequestError : ProblemDetails
{
    public BadRequestError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status400BadRequest;
        Detail = detail;
    }
}
