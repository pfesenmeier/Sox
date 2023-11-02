namespace Sox;

public record Token
{
    public required TokenType tokenType { get; init; }
    public required string lexeme { get; init; }
    public object? literal { get; init; }
    public required int line { get; init; }
}