namespace SharpLox
{
    internal class RuntimeError : Exception
    {
        internal Token Token { get; }

        internal RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}