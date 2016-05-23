using System;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class LispyStringVisitor : IVisitor
    {
        public string Result { get; private set; }

        public void Visit(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            string left = Result;
            expression.RightExpression.Accept(this);
            string right = Result;

            Result = string.Format("[{0} {1} {2}]",
                TokenTypeToString(expression.OperatorType),
                left,
                right);
        }

        public void Visit(ConstantExpression expression)
            => Result = expression.Value.ToString();

        public void Visit(ErrorExpression expression)
            => Result = string.Format("[Error {0}]", expression.Message);

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
