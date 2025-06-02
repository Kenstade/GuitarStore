using System.Security.Claims;
using BuildingBlocks.Core.Security.Authentication;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Security;

public interface IUserContext
{
    Guid GetUserId();
    Guid GetIdentityId();
}

public sealed class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid GetIdentityId()
    {
        var nameIdentifier = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(nameIdentifier, out var userId)
            ? userId
            : Guid.Empty; 
    }

    public Guid GetUserId()
    {
        var sub = accessor?.HttpContext?.User.FindFirstValue(CustomClaims.Sub);
        
        return Guid.TryParse(sub, out var userId)
            ? userId
            : Guid.Empty;
    }
}
