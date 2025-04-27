using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Identity.Errors;

public sealed class EmailAlreadyExistError : ConflictError
{
    public EmailAlreadyExistError() : base("Conflict", "The specified email already exist")
    {
    }
}