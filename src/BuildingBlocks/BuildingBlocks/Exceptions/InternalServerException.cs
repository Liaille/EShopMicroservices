namespace BuildingBlocks.Exceptions;

public class InternalServerException : Exception
{
    public string? Details { get; init; }

    public InternalServerException() : base()
    {
    }

    public InternalServerException(string message) : base(message)
    {
    }

    public InternalServerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InternalServerException(string message, string details) : base(message)
    {
        Details = details;
    }

}
