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
        return source.Count switch
        {
            > 1 => (source.Dequeue(), source.Peek()),
            1 => (source.Dequeue(), '\0'),
            _ => ('\0', '\0')
        };
    }

    private bool consumeString()
    {
        var str = string.Empty;
        while (source.Count > 0 && source.Peek() is not '"')
        {
            if (source.Peek() is '\n') line++;
            str += source.Dequeue();
        }

        if (source.Count is 0)
        {
            Program.error(line, "Unterminated string.");
            return source.Count > 0;
        }

        source.Dequeue();

        addToken(new TokenOptions(TokenType.STRING, literal: str, lexeme: $"\"{str}\""));

        return source.Count > 0;
    }

    private bool addToken(TokenOptions options)
    {
        tokens.Add(new Token
        {
            tokenType = options.tokenType,
            lexeme = options.lexeme,
            literal = options.literal,
            line = line
        });

        return source.Count > 0;
    }

    // pass in first char that has been removed from the queue
    private bool identifier(char first)
    {
        var text = first.ToString();

        while (source.Count > 0 && isAlphaNumeric(source.Peek())) text += source.Dequeue();

        if (keywords.ContainsKey(text))
            addToken(new TokenOptions(keywords[text], text));
        else
            addToken(new TokenOptions(TokenType.IDENTIFIER, text));

        return source.Count > 0;
    }

    private bool isAlpha(char c)
    {
        return char.IsAsciiLetter(c) || c is '_';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c)
    {
        return char.IsAsciiDigit(c);
    }

    // pass in the first digit that has already been taken from the queue
    private bool number(char first)
    {
        var text = "" + first;

        while (source.Count > 0 && isDigit(source.Peek())) text += source.Dequeue();

        if (source.Count > 1 && source.Peek() == '.' && isDigit(source.AsQueryable().Skip(1).First()))
        {
            text += source.Dequeue();

            while (source.Count > 0 && isDigit(source.Peek())) text += source.Dequeue();
        }

        addToken(new TokenOptions(TokenType.NUMBER, text, Convert.ToDouble(text)));

        return source.Count > 0;
    }

    private bool discardComment()
    {
        while (source.Count > 0 && source.Peek() is not '\n') source.Dequeue();

        return source.Count > 0;
    }

    private bool incrementLine()
    {
        line++;

        return source.Count > 0;
    }

    private bool consumeSecond()
    {
        source.Dequeue();

        return source.Count > 0;
    }

    private bool scanToken()
    {
        var match = next();

        Func<TokenType, bool> addOneTokenChar = type => addToken(new TokenOptions(type, match.first.ToString()));
        Func<TokenType, bool> addTwoTokenChar = type =>
        {
            consumeSecond();
            return addToken(new TokenOptions(type, "" + match.first + match.secord));
        };

        Func<char, bool> error = input =>
        {
            Program.error(line, $"unrecognized character: {input}");

            return source.Count > 0;
        };

        return match switch
        {
            ('(', _) => addOneTokenChar(TokenType.LEFT_PAREN),
            (')', _) => addOneTokenChar(TokenType.RIGHT_PAREN),
            ('{', _) => addOneTokenChar(TokenType.LEFT_BRACE),
            ('}', _) => addOneTokenChar(TokenType.RIGHT_BRACE),
            (',', _) => addOneTokenChar(TokenType.COMMA),
            ('.', _) => addOneTokenChar(TokenType.DOT),
            ('-', _) => addOneTokenChar(TokenType.MINUS),
            ('+', _) => addOneTokenChar(TokenType.PLUS),
            ('*', _) => addOneTokenChar(TokenType.STAR),
            (';', _) => addOneTokenChar(TokenType.SEMICOLON),
            ('!', '=') => addTwoTokenChar(TokenType.BANG_EQUAL),
            ('!', _) => addOneTokenChar(TokenType.BANG),
            ('=', '=') => addTwoTokenChar(TokenType.EQUAL_EQUAL),
            ('=', _) => addOneTokenChar(TokenType.EQUAL),
            ('>', '=') => addTwoTokenChar(TokenType.GREATER_EQUAL),
            ('>', _) => addOneTokenChar(TokenType.GREATER),
            ('<', '=') => addTwoTokenChar(TokenType.LESS_EQUAL),
            ('<', _) => addOneTokenChar(TokenType.LESS),
            ('/', '/') => consumeSecond() && discardComment(),
            ('/', _) => addOneTokenChar(TokenType.SLASH),
            ('"', _) => consumeString(),
            (' ' or '\r' or '\t', _) => source.Count > 0,
            ('\n', _) => incrementLine(),
            var (first, _) when isDigit(first) => number(first),
            var (first, _) when isAlpha(first) => identifier(first),
            var (first, _) => error(first)
        };
    }

    private record TokenOptions(TokenType tokenType, string lexeme, StringOrNumber? literal = null);
}