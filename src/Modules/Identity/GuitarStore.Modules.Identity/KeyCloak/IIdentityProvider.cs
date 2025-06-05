using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.Identity.KeyCloak;

public interface IIdentityProvider
{
    Task<Result<string>> RegisterUserAsync(string email, string firstName, string lastName, string password, 
        CancellationToken cancellationToken = default);
}