using System.Net;
using BuildingBlocks.Core.ErrorHandling;
using GuitarStore.Modules.Identity.Errors;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Modules.Identity.KeyCloak;

internal sealed class IdentityProvider(KeyCloakClient keyCloakClient, 
    ILogger<IdentityProvider> logger) : IIdentityProvider
{
    private const string PasswordCredentialType = "Password";
    
    public async Task<Result<string>> RegisterUserAsync(string email, string firstName, string lastName, string password, 
        CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation
        (
            email, 
            email, 
            firstName, 
            lastName, 
            true, 
            true, 
            [new CredentialRepresentation(PasswordCredentialType, password, false)]
        );

        try
        {
            string identityId = await keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);
            
            return identityId;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");
            return Result.Failure<string>(IdentityProviderErrors.EmailAlreadyExist(email));
        }
    }
}

internal sealed record UserRepresentation(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool EmailVerified,
    bool Enabled,
    CredentialRepresentation[] Credentials);
    
internal sealed record CredentialRepresentation(string Type, string Value, bool Temporary);