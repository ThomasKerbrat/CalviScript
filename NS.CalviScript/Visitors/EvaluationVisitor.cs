using System.Collections.Generic;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class EvaluationVisitor : StandardVisitor
    {
        private Dictionary<string, int> _globalContext;

        public EvaluationVisitor(Dictionary<string, int> globalContext)
        {
            _globalContext = globalContext;
        }

        public override IExpression Visit(BlockExpression expression)
        {
            IExpression last = UndefinedExpression.Default;
            foreach (var statement in expression.Statements)
            {
                last = statement.Accept(this);
            }
            return last;
        }

        public override IExpression Visit(LookUpExpression expression)
        {
            int value;
            if (_globalContext.TryGetValue(expression.Identifier, out value))
            {
                return new ConstantExpression(value);
            }
            else
            {
                return new ErrorExpression("Reference not found: " + expression.Identifier);
            }
        }

        public override IExpression Visit(BinaryExpression expression)
        {
            IExpression left = expression.LeftExpression.Accept(this);
            IExpression right = expression.RightExpression.Accept(this);

            if (left is UndefinedExpression)
                return left;
            if (right is UndefinedExpression)
                return right;

            if (left is ConstantExpression && right is ConstantExpression)
            {
                var constLeft = (ConstantExpression)left;
                var constRight = (ConstantExpression)right;
                switch (expression.OperatorType)
                {
                    case TokenType.Plus:
                        return new ConstantExpression(constLeft.Value + constRight.Value);
                    case TokenType.Minus:
                        return new ConstantExpression(constLeft.Value - constRight.Value);
                    case TokenType.Mult:
                        return new ConstantExpression(constLeft.Value * constRight.Value);
                    case TokenType.Div:
                        return new ConstantExpression(constLeft.Value / constRight.Value);
                    default:
                        Debug.Assert(expression.OperatorType == TokenType.Modulo);
                        return new ConstantExpression(constLeft.Value % constRight.Value);
                }
            }

            return left != expression.LeftExpression || right != expression.RightExpression
                ? new BinaryExpression(expression.OperatorType, left, right)
                : expression;
        }

        public override IExpression Visit(AssignExpression expression)
        {
            var result = expression.Expression.Accept(this);
            return result;
        }
    }
}
