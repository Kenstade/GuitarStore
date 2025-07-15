using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.ShoppingCart.Errors;

public static class CartErrors
{
    public static Error NotFound(Guid id) => Error.NotFound($"Shopping cart not found for user '{id}'.");
}