using GuitarStore.Common.Core.Exceptions.Types;

namespace GuitarStore.Catalogs.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message)
    {
    }
}
