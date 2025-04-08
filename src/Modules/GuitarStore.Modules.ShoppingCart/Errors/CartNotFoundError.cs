using BuildingBlocks.Web.Errors;

namespace GuitarStore.Modules.ShoppingCart.Errors;

internal sealed class CartNotFoundError : NotFoundError
{
    public CartNotFoundError(Guid customerId) : base("Not Found", $"Cart with customer Id '{customerId}' not found.")
    {
    }
}