namespace Sox;

public class Scanner
{
    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

    private readonly List<Token> tokens = new();

    private int line;

    private string text = string.Empty;

    public Scanner(string source)
    {
        this.source = new Queue<char>(source);
    }

    public Queue<char> source { get; }

    public List<Token> scanTokens()
    {
        while (scanToken())
        {
        }

        tokens.Add(new Token { tokenType = TokenType.EOF, lexeme = "", line = line });
        return tokens;
    }

    private (char first, char secord) next()
    {
        return source.Count() switch
        {
            > 1 => (source.Dequeue(), source.Peek()),
            1 => (source.Dequeue(), '\0'),
            _ => ('\0', '\0')
        };
    }

    private bool consumeString()
    {
        while (source.Peek() is not '"' or '\0')
        {
            if (source.Peek() is '\n') line++;
            text += source.Dequeue();
        }

        if (source.Peek() is '\0')
            // Lox.error(line, "Unterminated string.");
            return false;

        source.Dequeue();

        addToken(TokenType.STRING, text);
        text = string.Empty;

        return source.Count() > 0;
    }

    private bool addToken(TokenType type, object? literal = null)
    {
        tokens.Add(new Token
        {
            tokenType = type,
            literal = literal,
            line = line,
            lexeme = "TODO"
        });

        return source.Count > 0;
    }

    private bool consumeSecond()
    {
        source.Dequeue();

        return true;
    }

    private bool identifier()
    {
        while (isAlphaNumeric(source.Peek())) text += source.Dequeue();

        if (keywords.ContainsKey(text))
            addToken(keywords[text]);
        else
            addToken(TokenType.IDENTIFIER);

        text = string.Empty;

        return source.Count() > 0;
    }

    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool number()
    {
        while (isDigit(source.Peek())) text += source.Dequeue();

        if (source.Peek() == '.' && isDigit(source.AsQueryable().Skip(1).First()))
        {
            text += source.Dequeue();

            while (isDigit(source.Peek())) text += source.Dequeue();
        }

        addToken(TokenType.NUMBER, Convert.ToDouble(text));
        text = string.Empty;

        return source.Count > 0;
    }

    private bool discardComment()
    {
        while (source.Peek() is not '\n' or '\0') source.Dequeue();

        return source.Count > 0;
    }

    private bool incrementLine()
    {
        line++;

        return source.Count > 0;
    }

    private bool scanToken()
    {
        return next() switch
        {
            ('(', _) => addToken(TokenType.LEFT_PAREN),
            (')', _) => addToken(TokenType.RIGHT_PAREN),
            ('{', _) => addToken(TokenType.LEFT_BRACE),
            ('}', _) => addToken(TokenType.RIGHT_BRACE),
            (',', _) => addToken(TokenType.COMMA),
            ('.', _) => addToken(TokenType.DOT),
            ('-', _) => addToken(TokenType.MINUS),
            ('+', _) => addToken(TokenType.PLUS),
            ('*', _) => addToken(TokenType.STAR),
            (';', _) => addToken(TokenType.SEMICOLON),
            ('!', '=') => consumeSecond() && addToken(TokenType.BANG_EQUAL),
            ('!', _) => addToken(TokenType.BANG),
            ('=', '=') => consumeSecond() && addToken(TokenType.EQUAL_EQUAL),
            ('=', _) => addToken(TokenType.EQUAL),
            ('>', '=') => consumeSecond() && addToken(TokenType.GREATER_EQUAL),
            ('>', _) => addToken(TokenType.GREATER),
            ('<', '=') => consumeSecond() && addToken(TokenType.LESS_EQUAL),
            ('<', _) => addToken(TokenType.LESS),
            ('/', '/') => consumeSecond() && discardComment(),
            ('/', _) => addToken(TokenType.SLASH),
            ('"', _) => consumeString(),
            (' ' or '\r' or '\t', _) => source.Count > 0,
            ('\n', _) => incrementLine(),
            var (first, _) when isDigit(first) => number(),
            var (first, _) when isAlpha(first) => identifier(),
            (_, _) => throw new Exception()
        };
    }
}
//     List<Token> scanTokens() {
//         while (!isAtEnd()) {
//             start = current;
//             scanToken();
//         }
//
//         tokens.add(new Token(EOF, "", null, line));
//         return tokens;
//     }
//
//     private void string() {
//         while (peek() != '"' && !isAtEnd()) {
//            if (peek() == '\n') line++;
//            advance();
//         }
//
//         if (isAtEnd()){
//             Lox.error(line, "Unterminated string.");
//             return;
//         }
//
//         advance();
//
//         var value = source.substring(start + 1, current - 1);
//         addToken(STRING, value);
//     }
// }