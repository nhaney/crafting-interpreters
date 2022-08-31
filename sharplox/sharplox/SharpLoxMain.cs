namespace SharpLox
{
    public static class SharpLoxMain
    {
        private static bool s_hadError;

        public static void Main(string[] args)
        {
            // if (args.Length > 1)
            // {
            //     Console.WriteLine("Usage: sharplox [script]");
            // }
            // else if (args.Length == 1)
            // {
            //     RunFile(args[0]);
            // }
            // else
            // {
            //     RunPrompt();
            // }
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.Minus, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.Star, "*", null, 1),
                new Expr.Grouping(
                        new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));

            Expr rpnExpression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.Minus, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.Star, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal("str")));

            Console.WriteLine(new RpnPrinter().Print(rpnExpression));
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

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            s_hadError = true;
        }
    }
}