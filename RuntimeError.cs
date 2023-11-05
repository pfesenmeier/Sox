namespace Sox;

public class RuntimeError : Exception
{
    private readonly Token token;

    private RuntimeError(Token token, string message) : base(message)
    {
        this.token = token;
    }
}