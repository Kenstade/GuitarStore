using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Customers.Errors;

public class CustomerNotFoundError : NotFoundError
{
    public CustomerNotFoundError(Guid id) : base("Not Found", $"Customer with id '{id}' not found.")
    { }
}
