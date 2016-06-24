using System;
using System.Collections.Generic;
using System.Linq;

namespace NS.CalviScript
{
    public class StandardVisitor : IVisitor<IExpression>
    {
        public virtual IExpression Visit(LookUpExpression expression)
            => expression;

        public virtual IExpression Visit(BinaryExpression expression)
        {
            var left = expression.LeftExpression.Accept(this);
            var right = expression.RightExpression.Accept(this);
            return left != expression.LeftExpression
                || right != expression.RightExpression
                ? new BinaryExpression(expression.OperatorType, left, right)
                : expression;
        }

        public virtual IExpression Visit(VariableDeclarationExpression expression)
            => expression;

        public virtual IExpression Visit(AssignExpression expression)
        {
            IIdentifierExpression identifier = (IIdentifierExpression)expression.Identifier.Accept(this);
            IExpression _expression = expression.Expression.Accept(this);
            return identifier != expression.Identifier
                || _expression != expression.Expression
                ? new AssignExpression(identifier, _expression)
                : expression;
        }

        public IExpression Visit(UndefinedExpression expression)
            => expression;

        public virtual IExpression Visit(ErrorExpression expression)
            => expression;

        public virtual IExpression Visit(TernaryExpression expression)
        {
            IExpression predicate = expression.PredicateExpression.Accept(this);
            IExpression @true = expression.TrueExpression.Accept(this);
            IExpression @false = expression.FalseExpression.Accept(this);
            return predicate == expression.PredicateExpression
                || @true == expression.TrueExpression
                || @false == expression.FalseExpression
                ? new TernaryExpression(predicate, @true, @false)
                : expression;
        }

        public virtual IExpression Visit(UnaryExpression expression)
        {
            var possibleContantExpression = expression.Expression.Accept(this);

            if (possibleContantExpression is ConstantExpression)
                return new ConstantExpression(((ConstantExpression)possibleContantExpression).Value * -1);

            return possibleContantExpression;
        }

        public virtual IExpression Visit(ConstantExpression expression)
            => expression;

        public virtual IExpression Visit(BlockExpression expression)
        {
            List<IExpression> newBlock = null;
            var i = 0;

            foreach (var statement in expression.Statements)
            {
                var _statement = statement.Accept(this);
                if (_statement != statement)
                {
                    if (newBlock == null)
                    {
                        newBlock = new List<IExpression>();
                        newBlock.AddRange(expression.Statements.Take(1));
                    }
                }
                if (newBlock != null)
                {
                    newBlock.Add(_statement);
                }
                i++;
            }

            return newBlock != null
                ? new BlockExpression(newBlock)
                : expression;
        }

        public virtual IExpression Visit(WhileExpression expression)
        {
            IExpression condition = expression.Condition.Accept(this);
            BlockExpression body = (BlockExpression)expression.Body.Accept(this);
            return condition != expression.Condition || body != expression.Body
                ? new WhileExpression(condition, body)
                : expression;
        }

        public virtual IExpression Visit(FunctionDeclarationExpression expression)
        {
            List<VariableDeclarationExpression> possibleNewParameters = null;
            int i = 0;

            foreach ( var parameter in expression.Parameters)
            {
                var visitedParameter = parameter.Accept(this);
                if (visitedParameter != parameter)
                {
                    if (visitedParameter == null)
                    {
                        possibleNewParameters = new List<VariableDeclarationExpression>();
                        possibleNewParameters.AddRange(expression.Parameters.Take(i));
                    }
                }
                if (possibleNewParameters != null)
                    possibleNewParameters.Add((VariableDeclarationExpression)visitedParameter);
                ++i;
            }

            var newParameters = possibleNewParameters != null
                ? possibleNewParameters
                : expression.Parameters;
            var newBody = (BlockExpression)expression.Body.Accept(this);

            return newParameters != expression.Parameters || newBody != expression.Body
                ? new FunctionDeclarationExpression(newParameters, newBody)
                : expression;
        }
    }
}
