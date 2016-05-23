using System;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class BinaryExpression : IExpression
    {
        public BinaryExpression(TokenType type, IExpression left, IExpression right)
        {
            OperatorType = type;
            LeftExpression = left;
            RightExpression = right;
        }

        public TokenType OperatorType { get; set; }

        public IExpression LeftExpression { get; internal set; }

        public IExpression RightExpression { get; internal set; }

        public string ToLispyString() => string.Format(
            "[{0} {1} {2}]",
            OperatorTypeToString(),
            LeftExpression.ToLispyString(),
            RightExpression.ToLispyString());

        public string ToInfixString() => string.Format(
            "({0} {1} {2})",
            LeftExpression.ToInfixString(),
            OperatorTypeToString(),
            RightExpression.ToInfixString());

        string OperatorTypeToString()
        {
            if (OperatorType == TokenType.Plus) return "+";
            else if (OperatorType == TokenType.Minus) return "-";
            else if (OperatorType == TokenType.Mult) return "*";
            else if (OperatorType == TokenType.Div) return "/";
            else
            {
                Debug.Assert(OperatorType == TokenType.Modulo);
                return "%";
            }
        }
    }
}
