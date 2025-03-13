using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Customers.Errors;

public class AddressNotFoundError : NotFoundError
{
    public AddressNotFoundError() : base("Not Found", "Adress not found.")
    { }
}
