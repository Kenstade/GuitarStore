using BuildingBlocks.Core.Exceptions.Types;

namespace GuitarStore.Modules.Catalog.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message)
    {
    }
}
