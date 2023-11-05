using OneOf;
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
        return source.Count switch
        {
            > 1 => (source.Dequeue(), source.Peek()),
            1 => (source.Dequeue(), '\0'),
            _ => ('\0', '\0')
        };
    }

    private bool consumeString()
    {
        var str = String.Empty; 
        while (source.Count > 0 && source.Peek() is not '"')
        {
            if (source.Peek() is '\n') line++;
            str += source.Dequeue();
        }

        if (source.Count is 0)
            // Lox.error(line, "Unterminated string.");
            throw new Exception("Unterminated string");

        source.Dequeue();

        addToken(new(tokenType: TokenType.STRING, literal: str, lexeme: $"\"{str}\"" ));

        return source.Count > 0;
    }


    private record TokenOptions( TokenType tokenType,string lexeme, StringOrNumber? literal = null);
    private bool addToken(TokenOptions options)
    {
        tokens.Add(new() {
            tokenType = options.tokenType,
           lexeme = options.lexeme,
           literal = options.literal,
           line = line
            });

        return source.Count > 0;
    }

    private bool identifier()
    {
        // TODO text as local variable
        while (source.Count > 0 && isAlphaNumeric(source.Peek())) text += source.Dequeue();

        if (keywords.ContainsKey(text))
            addToken(new (tokenType: keywords[text], lexeme: text));
        else
            addToken(new (tokenType: TokenType.IDENTIFIER, lexeme: text));

        text = string.Empty;

        return source.Count > 0;
    }

    private bool isAlpha(char c)
    {
        return Char.IsAsciiLetter(c) || c is '_';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c)
    {
        return Char.IsAsciiDigit(c);
    }

    private bool number()
    {
        // TODO text local variable
        while (source.Count > 0 && isDigit(source.Peek())) text += source.Dequeue();

        if (source.Peek() == '.' && isDigit(source.AsQueryable().Skip(1).First()))
        {
            text += source.Dequeue();

            while (source.Count > 0 && isDigit(source.Peek())) text += source.Dequeue();
        }

        addToken(new (tokenType: TokenType.NUMBER, lexeme: text, literal: Convert.ToDouble(text)));
        text = string.Empty;

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

        Func<TokenType, bool> addOneTokenChar = (TokenType type) => addToken(new(tokenType: type, lexeme: match.first.ToString()));
        Func<TokenType, bool> addTwoTokenChar = (TokenType type) => {
          consumeSecond();
          return addToken(new(tokenType: type, lexeme: ""  + match.first + match.secord));
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
            // TODO throws
            var (first, _) when isDigit(first) => number(),
            // TODO cuts off first char
            var (first, _) when isAlpha(first) => identifier(),
            (_, _) => throw new Exception()
        };
    }
}

[GenerateOneOf]
public partial class StringOrChar: OneOfBase<string, char> {} 
