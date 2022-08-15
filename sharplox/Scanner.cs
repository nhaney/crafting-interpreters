using static SharpLox.TokenType;

namespace SharpLox
{
    internal class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new();
        private int _start;
        private int _current;
        private int _line = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
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
            _source = source;
        }

        internal List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(EOF, "", null, _line));
            return _tokens;
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
                    AddToken(Match('=') ? LessEqual : Less);
                    break;
                case '>':
                    AddToken(Match('=') ? GreaterEqual : Greater);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            _ = Advance();
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
                    _line++;
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
                        SharpLoxMain.Error(_line, $"Unexpected character {c}");
                    }
                    break;
            }
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            var text = _source[_start.._current];
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (_source[_current] != expected)
            {
                return false;
            }

            _current++;
            return true;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : _source[_current];
        }

        private char PeekNext()
        {
            return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
        }


        private static bool IsDigit(char c)
        {
            return c is >= '0' and <= '9';
        }

        private static bool IsAlpha(char c)
        {
            return c is (>= 'a' and <= 'z') or
                (>= 'A' and <= 'Z') or
                '_';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private void HandleNumber()
        {
            while (IsDigit(Peek()))
            {
                _ = Advance();
            }

            // Look for fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                _ = Advance();

                while (IsDigit(Peek()))
                {
                    _ = Advance();
                }
            }

            var number = _source[_start.._current];
            AddToken(LoxNumber, double.Parse(number));
        }

        private void HandleString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }
                _ = Advance();
            }

            if (IsAtEnd())
            {
                SharpLoxMain.Error(_line, "Unterminated String");
                return;
            }

            // The closing "
            _ = Advance();

            // Trim the surrounding quotes
            // Could also escape values here
            var value = _source[(_start + 1)..(_current - 1)];
            AddToken(LoxString, value);
        }

        private void HandleIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                _ = Advance();
            }

            var text = _source[_start.._current];
            if (!Keywords.TryGetValue(text, out TokenType type))
            {
                type = LoxIdentifier;
            }
            AddToken(type);
        }
    }
}