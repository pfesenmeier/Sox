namespace Sox;

public static class Interpreter
{
    private static bool IsEqual(object left, object right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) => false,
        _ => left.Equals(right),
    };

    public static object Evaluate(Expr expression) =>
        expression.Match(
            binary => (Evaluate(binary.left), binary.operation.tokenType, Evaluate(binary.right)) switch
            {
                (string left, TokenType.PLUS, string right) => left + right,

                (var left, TokenType.EQUAL_EQUAL, var right) => IsEqual(left, right),
                (var left, TokenType.BANG_EQUAL, var right) => !IsEqual(left, right),

                (double left, TokenType operation, double right) => operation switch
                {
                    TokenType.MINUS => left - right,
                    TokenType.STAR => left * right,
                    TokenType.SLASH => left / right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS => left < right,
                    TokenType.LESS_EQUAL => left <= right,
                    _ => throw new RuntimeException($"Operation not supported: {left} {operation} {right}"),

                },
                (var left, var operation, var right) => throw new RuntimeException($"Operation not supported: {left} {operation} {right}"),
            },
            unary => (unary.operation.tokenType, Evaluate(unary.right)) switch
            {
                (TokenType.MINUS, double right) => -right,
                // like Ruby: fales and null are falsey, everything else truthy
                (TokenType.BANG, bool right) => !right,
                (TokenType.BANG, null) => true,
                (TokenType.BANG, _) => false,
                (var operation, var right) => throw new RuntimeException($"Operation not supported: {operation}, {right}"),
            },
            literal => literal.value,
            grouping => Evaluate(grouping.expression));
}
