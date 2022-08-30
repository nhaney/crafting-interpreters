namespace SharpLox
{
    internal class Token
    {
        private readonly TokenType _type;
        private readonly string _lexeme;
        private readonly object? _literal;
        private readonly int _line;

        internal Token(TokenType type, string lexeme, object? literal, int line)
        {
            _type = type;
            _lexeme = lexeme;
            _literal = literal;
            _line = line;
        }

        public override string ToString()
        {
            return _type + " " + _lexeme + " " + _literal + " (line " + _line + ")";
        }
    }
}