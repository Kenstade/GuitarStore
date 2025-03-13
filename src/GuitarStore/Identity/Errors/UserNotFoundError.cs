using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Identity.Errors;

public class UserNotFoundError : NotFoundError
{
    public UserNotFoundError(string id) : base("Not Found", $"User with id: '{id}' not found.")
    {
    }
}
