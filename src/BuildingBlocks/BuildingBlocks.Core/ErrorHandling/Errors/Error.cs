using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Core.ErrorHandling.Errors;

public sealed class Error : ProblemDetails
{
    public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status400BadRequest);

    public static readonly Error NullValue = new("General.Null", "Null value was provided",
        StatusCodes.Status400BadRequest);

    public Error(string title, string detail, int statusCode)
    {
        Title = title;
        Detail = detail;
        Status = statusCode;
    }

    public static Error BadRequest(string title, string detail) => new(title, detail, StatusCodes.Status400BadRequest);

    public static Error NotFound(string title, string detail) => new(title, detail, StatusCodes.Status404NotFound);
    
    public static Error Unauthorized(string title, string detail) => new(title, detail, StatusCodes.Status401Unauthorized);
    
    public static Error Conflict(string title, string detail) => new(title, detail, StatusCodes.Status409Conflict);
}