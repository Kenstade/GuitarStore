using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Customers.Errors;

public class AddressNotFoundError : NotFoundError
{
    public AddressNotFoundError() : base("Not Found", "Adress not found.")
    { }
}
