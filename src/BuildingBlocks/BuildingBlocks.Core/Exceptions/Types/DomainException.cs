namespace BuildingBlocks.Core.Exceptions.Types;

public class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    { }

}
