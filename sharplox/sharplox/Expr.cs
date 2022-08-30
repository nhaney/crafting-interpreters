namespace SharpLox
{
    internal abstract class Expr
    {
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
        }

        internal class Grouping : Expr
        {
            internal Expr Expression;

            internal Grouping(Expr expression)
            {
                Expression = expression;
            }
        }

        internal class Literal : Expr
        {
            internal object Value;

            internal Literal(object value)
            {
                Value = value;
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
        }

    }
}
