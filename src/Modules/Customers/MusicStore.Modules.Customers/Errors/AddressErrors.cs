using BuildingBlocks.Core.ErrorHandling;

namespace MusicStore.Modules.Customers.Errors;

internal static class AddressErrors
{
    public static Error NotFound(Guid customerId) => Error.NotFound($"Address '{customerId}' not found.");
}