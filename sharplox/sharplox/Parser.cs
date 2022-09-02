using static SharpLox.TokenType;

namespace SharpLox
{
    internal class ParseError : Exception
    {
    }

    internal class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        internal Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public Expr? Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return CommaOperator();
        }

        private Expr CommaOperator()
        {
            Expr expr = Conditional();

            while (Match(Comma))
            {
                Token oper = Previous();
                Expr right = Conditional();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Conditional()
        {
            Expr expr = Equality();

            if (Match(QuestionMark))
            {
                Expr ifExpr = Expression();

                _ = Consume(Colon, "expect : after ? in ternary expression");

                Expr elseExpr = Conditional();

                expr = new Expr.Conditional(expr, ifExpr, elseExpr);
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(BangEqual, EqualEqual))
            {
                // grab the operator (which will be a != or ==)
                Token oper = Previous();

                // grab the right hand side
                Expr right = Comparison();

                // make the final binary expression using the last expression. This implies left associativity.
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(Greater, GreaterEqual, Less, LessEqual))
            {
                Token oper = Previous();
                Expr right = Term();

                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(Plus, Minus))
            {
                Token oper = Previous();
                Expr right = Factor();

                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(Slash, Star))
            {
                Token oper = Previous();
                Expr right = Unary();

                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(Bang, Minus))
            {
                Token oper = Previous();
                Expr right = Unary();
                return new Expr.Unary(oper, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(False))
            {
                return new Expr.Literal(false);
            }

            if (Match(True))
            {
                return new Expr.Literal(true);
            }

            if (Match(Nil))
            {
                return new Expr.Literal(null);
            }

            if (Match(LoxNumber, LoxString))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(LeftParen))
            {
                Expr expr = Expression();
                _ = Consume(RightParen, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private static ParseError Error(Token token, string message)
        {
            SharpLoxMain.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            _ = Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == Semicolon)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case Class:
                    case Fun:
                    case Var:
                    case For:
                    case If:
                    case While:
                    case Print:
                    case Return:
                        return;
                }

                _ = Advance();
            }
        }

        /// Checks to see if the current token matches any of the types passed into this methid
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    _ = Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        /// Checks if we have run out of tokens to parse
        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        /// Returns the current token
        private Token Peek()
        {
            return _tokens[_current];
        }

        /// Returns the previous token. Mostly used after `Match`.
        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}