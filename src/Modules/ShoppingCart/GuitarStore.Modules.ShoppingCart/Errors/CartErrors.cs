using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.ShoppingCart.Errors;

public static class CartErrors
{
    public static Error NotFound(Guid id) => Error.NotFound($"Shopping cart not found for user '{id}'.");
}