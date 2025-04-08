using BuildingBlocks.Core.Exceptions.Types;

namespace GuitarStore.Modules.Catalog.Exceptions;

internal class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message)
    {
    }
}
