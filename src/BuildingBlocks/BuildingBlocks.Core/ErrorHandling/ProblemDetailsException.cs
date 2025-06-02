namespace BuildingBlocks.Core.ErrorHandling;

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException(Error error, Exception? innerException = null) 
        : base(error.Detail, innerException)
    {
        Error = error;
    }
    
    public Error Error { get; }
}