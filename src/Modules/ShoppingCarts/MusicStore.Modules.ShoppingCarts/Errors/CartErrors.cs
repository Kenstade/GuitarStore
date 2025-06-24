using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.ShoppingCarts.Errors;

public static class CartErrors
{
    public static Error NotFound(Guid id) => Error.NotFound($"Shopping cart not found for user '{id}'.");
}