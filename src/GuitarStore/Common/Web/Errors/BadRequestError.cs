using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Common.Web.Errors;

public class BadRequestError : ProblemDetails
{
    public BadRequestError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status400BadRequest;
        Detail = detail;
    }
}
