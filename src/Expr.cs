using OneOf;

namespace Sox;

public record Binary(Expr left, Token operation, Expr right);

public record Unary(Token operation, Expr right);

public record Literal(object value);

public record Grouping(Expr expression);

public record MyNewRecord();

[GenerateOneOf]
public partial class Expr : OneOfBase<Binary, Unary, Literal, Grouping>
{
}