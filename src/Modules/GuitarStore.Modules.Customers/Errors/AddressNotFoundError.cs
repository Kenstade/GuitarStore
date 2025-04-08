using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Customers.Errors;

internal sealed class AddressNotFoundError : NotFoundError
{
    public AddressNotFoundError() : base("Not Found", "Adress not found.")
    { }
}
