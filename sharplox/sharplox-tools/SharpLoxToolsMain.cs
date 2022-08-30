namespace SharpLox
{
    public static class SharpLoxToolsMain
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: sharplox-tools <output directory>");
                Environment.Exit(65);
            }

            var outputDir = args[0];

            DefineAst(outputDir, "Expr", new List<string> {
                "Binary   : Expr left, Token oper, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token oper, Expr right"}
            );
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            var path = outputDir + "/" + baseName + ".cs";

            using var writer = new StreamWriter(path);

            writer.WriteLine("namespace SharpLox");
            writer.WriteLine("{");
            writer.WriteLine("    internal abstract class " + baseName + "");
            writer.WriteLine("    {");

            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
                writer.WriteLine();
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine("        internal class " + className + " : " + baseName);
            writer.WriteLine("        {");

            var fields = fieldList.Split(", ");

            // fields
            foreach (var field in fields)
            {
                var type = field.Split(" ")[0];
                var name = field.Split(" ")[1];

                writer.WriteLine("            internal " + type + " " + char.ToUpper(name[0]) + name[1..] + ";");
            }

            writer.WriteLine();

            // Constructor
            writer.WriteLine("            internal " + className + "(" + fieldList + ")");
            writer.WriteLine("            {");


            // Store parameters in fields
            foreach (var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine("                " + char.ToUpper(name[0]) + name[1..] + " = " + name + ";");
            }
            writer.WriteLine("            }");
            writer.WriteLine("        }");
        }
    }
}