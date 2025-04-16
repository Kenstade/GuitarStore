using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Identity.Errors;

public class UserNotFoundError : NotFoundError
{
    public UserNotFoundError(string id) : base("Not Found", $"User with id: '{id}' not found.")
    {
    }
}
