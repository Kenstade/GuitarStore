using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.Identity.Errors;

public static class IdentityProviderErrors
{
    public static Error EmailAlreadyExist(string email) => Error.Conflict($"Email '{email}' already exist.");
}