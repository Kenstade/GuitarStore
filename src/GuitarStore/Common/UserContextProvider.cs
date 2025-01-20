using System.Security.Claims;

namespace GuitarStore.Common;

public interface IUserContextProvider
{
    public Guid GetUserId();
    public bool IsAuthenticated();
}

public class UserContextProvider(IHttpContextAccessor accessor) : IUserContextProvider
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid GetUserId()
    {
        var nameIndentifier = _accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(nameIndentifier, out var userId);

        return userId;
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext.User.Identity.IsAuthenticated;
    }
}
