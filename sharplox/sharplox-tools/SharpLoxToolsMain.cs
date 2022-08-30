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
                "Literal  : object? value",
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

            DefineVisitor(writer, baseName, types);
            writer.WriteLine();

            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
                writer.WriteLine();
            }

            writer.WriteLine("        internal abstract R Accept<R>(IVisitor<R> visitor);");

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

                writer.WriteLine("            internal " + type + " " + Capitalize(name) + ";");
            }

            writer.WriteLine();

            // Constructor
            writer.WriteLine("            internal " + className + "(" + fieldList + ")");
            writer.WriteLine("            {");


            // Store parameters in fields
            foreach (var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine("                " + Capitalize(name) + " = " + name + ";");
            }
            writer.WriteLine("            }");

            // Visitor pattern implementation
            writer.WriteLine();
            writer.WriteLine("            internal override R Accept<R>(IVisitor<R> visitor)");
            writer.WriteLine("            {");
            writer.WriteLine("                return visitor.Visit" + className + baseName + "(this);");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
        }


        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("        internal interface IVisitor<R>");
            writer.WriteLine("        {");

            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();

                writer.WriteLine("            R Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
            }
            writer.WriteLine("        }");
        }

        private static string Capitalize(string word)
        {
            return word == null ? throw new ArgumentNullException(word) : char.ToUpper(word[0]) + word[1..];
        }
    }
}