using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Core.ErrorHandling.Errors;

public sealed class Error : ProblemDetails
{
    public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status400BadRequest);

    public static readonly Error NullValue = new("General.Null", "Null value was provided",
        StatusCodes.Status400BadRequest);

    private Error(string title, string detail, int statusCode)
    {
        Title = title;
        Detail = detail;
        Status = statusCode;
    }

    public static Error BadRequest(string detail) => new("Bad Request", detail, StatusCodes.Status400BadRequest);

    public static Error NotFound(string detail) => new("Not found", detail, StatusCodes.Status404NotFound);
    
    public static Error Unauthorized(string detail) => new("Unauthorized", detail, StatusCodes.Status401Unauthorized);
    public static Error Forbidden(string detail) => new("Forbidden", detail, StatusCodes.Status403Forbidden);
    
    public static Error Conflict(string detail) => new("Conflict", detail, StatusCodes.Status409Conflict);
}