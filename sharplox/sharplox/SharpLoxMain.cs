namespace SharpLox
{
    public static class SharpLoxMain
    {
        private static bool s_hadError;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: sharplox [script]");
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            var contents = File.ReadAllText(path);
            Run(contents);

            if (s_hadError)
            {
                Environment.Exit(65);
            }
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line == null)
                {
                    break;
                }

                Run(line);
                s_hadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Console.WriteLine("Tokens: ");
            Console.WriteLine("*********************************************************");
            foreach (Token token in tokens)
            {
                Console.Write($"'{token}', ");
            }
            Console.Write("\n");
            Console.WriteLine("*********************************************************");

            var parser = new Parser(tokens);
            Expr? expression = parser.Parse();

            if (expression == null)
            {
                Console.WriteLine("Parser did not produce a valid expression...");
                return;
            }
            Console.WriteLine("AST: ");
            Console.WriteLine(new AstPrinter().Print(expression));
            Console.WriteLine("*********************************************************");
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            s_hadError = true;
        }
    }
}