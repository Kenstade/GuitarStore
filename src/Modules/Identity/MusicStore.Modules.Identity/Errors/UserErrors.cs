using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.Identity.Errors;

public static class UserErrors
{
    public static Error NotFound(string identityId) => Error.NotFound($"User with Identity ID '{identityId}' not found.");
}