using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.Order.Errors;

public static class OrderErrors
{
    public static Error NotFound(string id) => Error.NotFound($"Order '{id}' not found.");
}