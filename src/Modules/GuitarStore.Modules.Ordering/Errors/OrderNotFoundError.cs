using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.Ordering.Errors;

public class OrderNotFoundError : NotFoundError
{
    public OrderNotFoundError(Guid id) : base("Not Found", $"Order with id '{id}' not found.")
    {
    }
}
