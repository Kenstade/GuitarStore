using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Common.Web.Errors;

public class NotFoundError : ProblemDetails
{
    protected NotFoundError(string title, string detail)
    {
        Title = title;
        Status = StatusCodes.Status404NotFound;
        Detail = detail;
    }
}
