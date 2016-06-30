using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NS.CalviScript
{
    public class EvaluationVisitor : IVisitor<BaseValue>
    {
        private Dictionary<string, BaseValue> _globalContext;
        private Dictionary<VariableDeclarationExpression, BaseValue> _variables;

        public EvaluationVisitor(Dictionary<string, BaseValue> globalContext)
        {
            _globalContext = globalContext;
            _variables = new Dictionary<VariableDeclarationExpression, BaseValue>();
        }

        public BaseValue Visit(BlockExpression expression)
        {
            BaseValue last = UndefinedValue.Default;
            foreach (var statement in expression.Statements)
            {
                last = statement.Accept(this);
            }
            return last;
        }

        public BaseValue Visit(LookUpExpression expression)
        {
            if (expression.VariableDeclaration != null)
            {
                return _variables[expression.VariableDeclaration];
            }
            else
            {
                BaseValue value;
                if (_globalContext.TryGetValue(expression.Identifier, out value))
                {
                    return value;
                }
                return new ErrorValue("Reference not found: " + expression.Identifier);
            }

        }

        public BaseValue Visit(UnaryExpression expression)
        {
            var result = expression.Expression.Accept(this);
            if (!(result is IntegerValue))
                return UndefinedValue.Default;
            var value = (IntegerValue)result;
            return IntegerValue.Create(-value.Value);
        }

        public BaseValue Visit(BinaryExpression expression)
        {
            var left = expression.LeftExpression.Accept(this);
            var right = expression.RightExpression.Accept(this);

            if (left is IntegerValue && right is IntegerValue)
            {
                var constLeft = (IntegerValue)left;
                var constRight = (IntegerValue)right;
                switch (expression.OperatorType)
                {
                    case TokenType.Plus:
                        return IntegerValue.Create(constLeft.Value + constRight.Value);
                    case TokenType.Minus:
                        return IntegerValue.Create(constLeft.Value - constRight.Value);
                    case TokenType.Mult:
                        return IntegerValue.Create(constLeft.Value * constRight.Value);
                    case TokenType.Div:
                        return IntegerValue.Create(constLeft.Value / constRight.Value);
                    default:
                        Debug.Assert(expression.OperatorType == TokenType.Modulo);
                        return IntegerValue.Create(constLeft.Value % constRight.Value);
                }
            }

            return UndefinedValue.Default;
        }

        public BaseValue Visit(TernaryExpression expression)
        {
            var result = expression.PredicateExpression.Accept(this);
            var value = result as IntegerValue;
            if (value != null)
            {
                return value.Value >= 0
                    ? expression.TrueExpression.Accept(this)
                    : expression.FalseExpression.Accept(this);
            }
            return UndefinedValue.Default;
        }

        public BaseValue Visit(VariableDeclarationExpression expression)
        {
            _variables.Add(expression, UndefinedValue.Default);
            return UndefinedValue.Default;
        }

        public BaseValue Visit(AssignExpression expression)
        {
            var result = expression.Expression.Accept(this);
            if (expression.Identifier.VariableDeclaration != null)
            {
                return _variables[expression.Identifier.VariableDeclaration] = result;
            }
            else
            {
                // if (strictMode)
                // return new ErrorValue("Global assignation is disabled in strict mode.");
                return _globalContext[expression.Identifier.Identifier] = result;
            }
        }

        public BaseValue Visit(UndefinedExpression expression)
        {
            return UndefinedValue.Default;
        }

        public BaseValue Visit(ErrorExpression expression)
        {
            return new ErrorValue(expression.Message);
        }

        public BaseValue Visit(ConstantExpression expression)
        {
            return IntegerValue.Create(expression.Value);
        }

        public BaseValue Visit(WhileExpression expression)
        {
            BaseValue result = UndefinedValue.Default;

            while (expression.Condition.Accept(this).IsTrue)
            {
                result = expression.Body.Accept(this);
            }

            return result;
        }

        public BaseValue Visit(FunctionDeclarationExpression expression)
        {
            return UndefinedValue.Default;
        }

        public BaseValue Visit(FunctionCallExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
