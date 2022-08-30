namespace SharpLox
{
    internal abstract class Expr
    {
        internal interface IVisitor<R>
        {
            R VisitBinaryExpr(Binary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
        }

        internal class Binary : Expr
        {
            internal Expr Left;
            internal Token Oper;
            internal Expr Right;

            internal Binary(Expr left, Token oper, Expr right)
            {
                Left = left;
                Oper = oper;
                Right = right;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        internal class Grouping : Expr
        {
            internal Expr Expression;

            internal Grouping(Expr expression)
            {
                Expression = expression;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        internal class Literal : Expr
        {
            internal object? Value;

            internal Literal(object? value)
            {
                Value = value;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        internal class Unary : Expr
        {
            internal Token Oper;
            internal Expr Right;

            internal Unary(Token oper, Expr right)
            {
                Oper = oper;
                Right = right;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        internal abstract R Accept<R>(IVisitor<R> visitor);
    }
}
