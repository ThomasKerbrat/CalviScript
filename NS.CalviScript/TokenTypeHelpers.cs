using System.Diagnostics;

namespace NS.CalviScript
{
    internal static class TokenTypeHelpers
    {
        internal static string TokenTypeToString(TokenType type)
        {
            if (type == TokenType.Plus) return "+";
            else if (type == TokenType.Minus) return "-";
            else if (type == TokenType.Mult) return "*";
            else if (type == TokenType.Div) return "/";
            else
            {
                Debug.Assert(type == TokenType.Modulo);
                return "%";
            }
        }
    }
}
