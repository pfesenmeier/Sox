namespace Sox;

public class Program
{
    private static bool hadError;

    public static void Main(string[] args)
    {
        if (args.Count() > 1)
        {
            Console.WriteLine("Usage: lox [script]");
            Environment.Exit(64);
        }

        // "dotnet watch" passes "run" as argument...
        if (args.Count() == 1 && args[0] != "run")
            runFile(args[0]);
        else
            runPrompt();
    }

    private static void runPrompt()
    {
        Console.WriteLine("Welcome to Lox v0.0.1.");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) break;
            run(line);
            hadError = false;
        }
    }

    private static void runFile(string path)
    {
        var file = File.ReadAllText(path);

        run(file);

        if (hadError) Environment.Exit(65);
    }

    private static void run(string source)
    {
        var scanner = new Scanner(source);

        var tokens = scanner.scanTokens();

        foreach (var token in tokens) info(token);
        // Parser parser = new Parser(tokens);
        // Expr expression = parser.parse();
        // if (hadError) return;
        //
        // Console.WriteLine(new AstPrinter().print(expression));
    }

    public static void error(int line, string message)
    {
        report(line, "", message);
        hadError = true;
    }

    private static void report(int line, string where, string message)
    {
        print($"[line {line}] Error {where}: {message}", ConsoleColor.DarkMagenta);
    }

    private static void print(string message, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = prev;
    }

    private static void info(object message)
    {
        print(message?.ToString() ?? "", ConsoleColor.Green);
    }


    public static void error(Token token, string message)
    {
        if (token.tokenType == TokenType.EOF)
            report(token.line, "at end", message);
        else
            report(token.line, "at '" + token.lexeme + "'", message);
    }
}