using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Orders.Errors;

public class OrderNotFoundError : NotFoundError
{
    public OrderNotFoundError(Guid id) : base("Not Found", $"Order with id '{id}' not found.")
    {
    }
}
