using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.CalviScript
{
    public class EvaluationVisitor : IVisitor
    {
        public EvaluationVisitor() { }

        public int Result { get; private set; }

        public void Visit(ErrorExpression expression)
        {
            throw new InvalidOperationException(expression.Message);
        }

        public void Visit(ConstantExpression expression)
            => Result = expression.Value;

        public void Visit(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            int left = Result;
            expression.RightExpression.Accept(this);
            int right = Result;
            Compute(left, right, expression.OperatorType);
        }

        private void Compute(int left, int right, TokenType operatorType)
        {
            switch (operatorType)
            {
                case TokenType.Plus:
                    Result = left + right;
                    break;
                case TokenType.Minus:
                    Result = left - right;
                    break;
                case TokenType.Mult:
                    Result = left * right;
                    break;
                case TokenType.Div:
                    Result = left / right;
                    break;
                case TokenType.Modulo:
                    Result = left % right;
                    break;
                default:
                    throw new ArgumentException("Not an operator.");
            }
        }
    }
}
