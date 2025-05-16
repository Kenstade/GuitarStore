using BuildingBlocks.Core.Exceptions.Types;

namespace GuitarStore.Modules.Catalog.Exceptions;

internal sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message)
    {
    }
}
