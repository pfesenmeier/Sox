namespace Sox;

public sealed class RuntimeException : Exception
{
    public RuntimeException()
    {
    }

    public RuntimeException(string? message) : base(message)
    {
    }

    public RuntimeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
