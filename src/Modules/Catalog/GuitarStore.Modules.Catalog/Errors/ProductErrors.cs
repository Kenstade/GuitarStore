using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.Catalog.Errors;

public static class ProductErrors
{
    public static Error NotFound(string productId) => Error.NotFound($"Product with ID '{productId}' not found.");
    public static Error NotFound(Guid productId) => Error.NotFound($"Product with ID '{productId}' not found.");

    public static Error InsufficientStock(Guid id, int quantity) =>
        Error.Conflict($"Empty stock, product '{id}' with quantity '{quantity}' is not available.");
}