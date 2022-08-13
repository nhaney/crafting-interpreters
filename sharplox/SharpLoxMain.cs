namespace SharpLox
{
    public static class SharpLoxMain {
        static bool HadError = false;

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

            if (HadError)
            {
                System.Environment.Exit(65);
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
                HadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);

            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
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
            HadError = true;
        }
    }
}