namespace SharpLox
{
    using static TokenType;
    internal class Scanner 
    {
        private readonly string Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int Start = 0;
        private int Current = 0;
        private int Line = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType> {
            {"and", And},
            {"class", Class},
            {"else", Else},
            {"false", False},
            {"for", For},
            {"fun", Fun},
            {"if", If},
            {"nil", Nil},
            {"or", Or},
            {"print", Print},
            {"return", Return},
            {"super", Super},
            {"this", This},
            {"true", True},
            {"var", Var},
            {"while", While},
        };

        internal Scanner(string source)
        {
            this.Source = source;
        }

        internal List<Token> ScanTokens() 
        {
            while (!IsAtEnd())
            {
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case '(': AddToken(LeftParen); break;
                case ')': AddToken(RightParen); break;
                case '{': AddToken(LeftBrace); break;
                case '}': AddToken(RightBrace); break;
                case ',': AddToken(Comma); break;
                case '.': AddToken(Dot); break;
                case '-': AddToken(Minus); break;
                case '+': AddToken(Plus); break;
                case ';': AddToken(Semicolon); break;
                case '*': AddToken(Star); break;
                case '!':
                    AddToken(Match('=') ? BangEqual : Bang);
                    break;
                case '=':
                    AddToken(Match('=') ? EqualEqual : Equal);
                    break;
                case '<':
                    AddToken(Match('=') ? LessEqual: Less);
                    break;
                case '>':
                    AddToken(Match('=') ? GreaterEqual: Greater);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(Slash);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    Line++;
                    break;
                case '"':
                    HandleString();
                    break;
                case 'o':
                    if (Match('r'))
                    {
                        AddToken(Or);
                    }
                    break;
                default:
                    if (IsDigit(c))
                    {
                        HandleNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        HandleIdentifier();
                    }
                    else
                    {
                        SharpLoxMain.Error(Line, $"Unexpected character {c}");
                    }
                    break;
            }
        }

        private bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        private char Advance()
        {
            return Source[Current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            var text = Source.Substring(Start, Current - Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (Source[Current] != expected)
            {
                return false;
            }

            Current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }

            return Source[Current];
        }

        private char PeekNext()
        {
            if (Current + 1 >= Source.Length)
            {
                return '\0';
            }

            return Source[Current + 1];
        }


        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private void HandleNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            // Look for fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            var number = Source.Substring(Start, Current - Start);
            AddToken(Number, Double.Parse(number));
        }

        private void HandleString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    Line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                SharpLoxMain.Error(Line, "Unterminated String");
                return;
            }

            // The closing "
            Advance();

            // Trim the surrounding quotes
            // Could also escape values here
            string value = Source.Substring(Start + 1, (Current - 1) - (Start + 1));
            AddToken(String, value);
        }

        private void HandleIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            var text = Source.Substring(Start, Current - Start);
            if (!Keywords.TryGetValue(text, out var type))
            {
                type = Identifier;
            }
            AddToken(type);
        }
    }
}