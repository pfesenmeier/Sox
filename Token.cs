using OneOf;

namespace Sox;

public record Token
{
    public required TokenType tokenType { get; init; }
    public required string lexeme { get; init; }
    public StringOrNumber? literal { get; init; }
    public required int line { get; init; }

    public override string ToString()
    {
        return $"{line}: {literal ?? lexeme}\t({tokenType})";
    }
}

[GenerateOneOf]
public partial class StringOrNumber : OneOfBase<string, double>
{
    public override string ToString()
    {
        return Value.ToString() ?? "";
    }
}