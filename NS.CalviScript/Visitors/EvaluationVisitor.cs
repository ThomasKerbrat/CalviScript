using System;

namespace NS.CalviScript
{
    public class EvaluationVisitor : IVisitor
    {
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

    public class GenericEvaluationVisitor : IVisitor<int>
    {
        public int Visit(ErrorExpression expression)
        {
            throw new InvalidOperationException(expression.Message);
        }

        public int Visit(ConstantExpression expression)
            => expression.Value;

        public int Visit(BinaryExpression expression)
        {
            switch (expression.OperatorType)
            {
                case TokenType.Plus:
                    return expression.LeftExpression.Accept(this) + expression.RightExpression.Accept(this);
                case TokenType.Minus:
                    return expression.LeftExpression.Accept(this) - expression.RightExpression.Accept(this);
                case TokenType.Mult:
                    return expression.LeftExpression.Accept(this) * expression.RightExpression.Accept(this);
                case TokenType.Div:
                    return expression.LeftExpression.Accept(this) / expression.RightExpression.Accept(this);
                case TokenType.Modulo:
                    return expression.LeftExpression.Accept(this) % expression.RightExpression.Accept(this);
                default:
                    throw new ArgumentException("Not an operator");
            }
        }
    }
}
