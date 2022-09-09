using static SharpLox.TokenType;

namespace SharpLox
{
    internal class Interpreter : Expr.IVisitor<object>
    {
        internal void Interpret(Expr expr)
        {
            try
            {
                object value = Evaluate(expr);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError e)
            {
                SharpLoxMain.RuntimeError(e);
            }
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type)
            {
                case Bang:
                    return !IsTruthy(right);
                case Minus:
                    CheckNumberOperand(expr.Oper, right);
                    return -(double)right;
            }

            return null;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type)
            {
                case Greater:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left > (double)right;
                case GreaterEqual:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left >= (double)right;
                case Less:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left < (double)right;
                case LessEqual:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left <= (double)right;
                case Minus:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left - (double)right;
                case BangEqual:
                    return !IsEqual(left, right);
                case EqualEqual:
                    return IsEqual(left, right);
                case Plus:
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                    {
                        return (double)left + (double)right;
                    }

                    if (left.GetType() == typeof(string) || right.GetType() == typeof(string))
                    {
                        return Stringify(left) + Stringify(right);
                    }

                    throw new RuntimeError(expr.Oper, "Operands must be two numbers or two strings.");
                case Slash:
                    CheckNumberOperands(expr.Oper, left, right);
                    if ((double)right == 0.0)
                    {
                        throw new RuntimeError(expr.Oper, "Divide by zero error.");
                    }
                    return (double)left / (double)right;
                case Star:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left * (double)right;
            }

            return null;
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private static bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(bool))
            {
                return (bool)obj;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token oper, object operand)
        {
            if (operand.GetType() == typeof(double))
            {
                return;
            }

            throw new RuntimeError(oper, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token oper, object left, object right)
        {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
            {
                return;
            }

            throw new RuntimeError(oper, "Operands must be numbers.");
        }

        private static string Stringify(object obj)
        {
            if (obj == null)
            {
                return "nil";
            }

            if (obj.GetType() == typeof(double))
            {
                string text = obj.ToString();

                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return obj.ToString();
        }
    }
}