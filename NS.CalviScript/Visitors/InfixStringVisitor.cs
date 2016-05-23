using System;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class InfixStringVisitor : IVisitor
    {
        public string Result { get; private set; }

        public void VisitBinaryExpression(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            string left = Result;
            expression.RightExpression.Accept(this);
            string right = Result;

            Result = string.Format("({0} {1} {2})",
                left,
                TokenTypeToString(expression.OperatorType),
                right);
        }

        public void VisitConstantExpression(ConstantExpression expression)
            => Result = expression.Value.ToString();

        public void VisitErrorExpression(ErrorExpression expression)
            => string.Format("[Error {0}]", expression.Message);

        // TODO: Remove this shit.
        string TokenTypeToString(TokenType type)
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
