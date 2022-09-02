using System.Text;

namespace SharpLox
{
    internal class AstPrinter : Expr.IVisitor<string>
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
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            if (expr.Value == null)
            {
                return "nil";
            }

            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Oper.Lexeme, expr.Right);
        }

        public string VisitConditionalExpr(Expr.Conditional expr)
        {
            return Parenthesize(":?", expr.Condition, expr.IfExpr, expr.ElseExpr);
        }

        internal string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder(0);

            _ = sb.Append('(').Append(name);

            foreach (Expr expr in exprs)
            {
                _ = sb.Append(' ');
                _ = sb.Append(expr.Accept(this));
            }

            _ = sb.Append(')');

            return sb.ToString();
        }
    }
}