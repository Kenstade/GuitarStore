using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Identity.Errors;

public sealed class InvalidCredentialsError : UnauthorizedError
{
    public InvalidCredentialsError() : base("Invalid Credentials", "Incorrect e-mail or password")
    { }
}
