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

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
