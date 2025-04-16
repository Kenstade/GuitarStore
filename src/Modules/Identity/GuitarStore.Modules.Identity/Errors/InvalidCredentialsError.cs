using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Identity.Errors;

public sealed class InvalidCredentialsError : UnauthorizedError
{
    public InvalidCredentialsError() : base("Invalid Credentials", "Incorrect e-mail or password")
    { }
}
