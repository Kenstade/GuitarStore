using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Customers.Errors;

public class CustomerNotFoundError : NotFoundError
{
    public CustomerNotFoundError(Guid id) : base("Not Found", $"Customer with id '{id}' not found.")
    { }
}
