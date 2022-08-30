using System.Text;

namespace SharpLox
{
    internal class AstPrinter : Expr.IVisitor<String>
    {
        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value == null ? "nil" : expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Oper.Lexeme, expr.Right);
        }

        internal string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder(0);

            sb.Append("(").Append(name);

            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}