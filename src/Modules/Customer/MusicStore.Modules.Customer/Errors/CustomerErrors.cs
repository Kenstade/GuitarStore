using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.Customer.Errors;

public static class CustomerErrors
{
    public static Error NotFound(Guid customerId) => Error.NotFound($"Customer '{customerId}' not found.");
}