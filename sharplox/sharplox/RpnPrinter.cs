namespace SharpLox
{
    internal class RpnPrinter : Expr.IVisitor<string>
    {

        internal string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return expr.Left.Accept(this) + " " + expr.Right.Accept(this) + " " + expr.Oper.Lexeme;
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return expr.Expression.Accept(this);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value == null ? "nil" : expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            var oper = expr.Oper.Lexeme;

            if (expr.Oper.Type == TokenType.Minus)
            {
                oper = "~";
            }

            return expr.Right.Accept(this) + " " + oper;
        }
    }
}