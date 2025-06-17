using BuildingBlocks.Core.ErrorHandling;

namespace GuitarStore.Modules.Orders.Errors;

public static class OrderErrors
{
    public static Error NotFound(string id) => Error.NotFound($"Order '{id}' not found.");
}