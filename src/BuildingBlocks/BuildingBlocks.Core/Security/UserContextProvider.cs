using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Security;

public interface IUserContextProvider
{
    Guid GetUserId();
}

public class UserContextProvider(IHttpContextAccessor accessor) : IUserContextProvider
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid GetUserId()
    {
        var nameIndentifier = _accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(nameIndentifier, out var userId)
            ? userId
            : Guid.Empty;
    }
}
