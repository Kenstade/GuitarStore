using System.Security.Claims;
using BuildingBlocks.Core.ErrorHandling;

namespace BuildingBlocks.Core.Security.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {                                           
        string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new ProblemDetailsException(Error.Unauthorized("Sub claim is missing"));
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               throw new ProblemDetailsException(Error.Unauthorized("NameIdentifier claim is missing"));
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {                                                               
        IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
                                              throw new ProblemDetailsException(Error.Forbidden("Permission claims are missing"));

        return permissionClaims.Select(x => x.Value).ToHashSet();
    }
}