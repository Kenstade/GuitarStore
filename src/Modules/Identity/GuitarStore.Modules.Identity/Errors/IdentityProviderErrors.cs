using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.Identity.Errors;

public static class IdentityProviderErrors
{
    
    public static Error EmailAlreadyExist(string email) => Error.Conflict($"Email '{email}' already exist.");
}