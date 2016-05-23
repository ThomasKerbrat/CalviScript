using System;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class BinaryExpression : IExpression
    {
        public BinaryExpression(TokenType type, IExpression left, IExpression right)
        {
            TokenType = type;
            LeftExpression = left;
            RightExpression = right;
        }

        public TokenType TokenType { get; set; }

        public IExpression LeftExpression { get; internal set; }

        public IExpression RightExpression { get; internal set; }

        public string ToLispyString() => string.Format(
            "[{0} {1} {2}]",
            TokenTypeToString(),
            LeftExpression.ToLispyString(),
            RightExpression.ToLispyString());

        string TokenTypeToString()
        {
            if (TokenType == TokenType.Plus) return "+";
            else if (TokenType == TokenType.Minus) return "-";
            else if (TokenType == TokenType.Mult) return "*";
            else if (TokenType == TokenType.Div) return "/";
            else
            {
                Debug.Assert(TokenType == TokenType.Modulo);
                return "%";
            }
        }
    }
}
