using System.Security.Claims;

namespace BuildingBlocks.Core.Security.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {                                           
        string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId
                                                            : throw new InvalidOperationException("No userId claim found"); 
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               throw new InvalidOperationException("No IdentityId claim found");
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {                                                               
        IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
                                              throw new InvalidOperationException("No permission claims found");

        return permissionClaims.Select(x => x.Value).ToHashSet();
    }
}