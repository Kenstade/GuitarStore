using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Orders.Errors;

public class CartNotFoundError : NotFoundError
{
    public CartNotFoundError(Guid id) : base("Not found", $"Cart with userId '{id}' not found.")
    {
    }
}
