using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NS.CalviScript
{
    public class EvaluationVisitor : IVisitor<BaseValue>
    {
        private Dictionary<string, BaseValue> _globalContext;
        private DynamicScope _dynamicScope;

        public EvaluationVisitor(Dictionary<string, BaseValue> globalContext)
        {
            _globalContext = globalContext;
            _dynamicScope = new DynamicScope();
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
                return _dynamicScope.FindRegistered(expression.VariableDeclaration);
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
            _dynamicScope.Register(expression, UndefinedValue.Default);
            return UndefinedValue.Default;
        }

        public BaseValue Visit(AssignExpression expression)
        {
            var result = expression.Expression.Accept(this);
            if (expression.Identifier.VariableDeclaration != null)
            {
                return _dynamicScope.SetValue(expression.Identifier.VariableDeclaration, result);
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
                using (_dynamicScope.OpenScope())
                {
                    result = expression.Body.Accept(this);
                }
            }

            return result;
        }

        public BaseValue Visit(FunctionDeclarationExpression expression)
        {
            return new FunctionValue(expression);
        }

        public BaseValue Visit(FunctionCallExpression expression)
        {
            List<BaseValue> arguments = expression.Arguments.Select(a => a.Accept(this)).ToList();

            BaseValue possibleBody = expression.Name.Accept(this);
            FunctionValue body = possibleBody as FunctionValue;
            if (body == null)
                return new ErrorValue($"{expression.Name.Identifier} is not a function.");

            // Adding undefined values for not passed arguments.
            while (body.FunctionDeclaration.Parameters.Count > arguments.Count)
            {
                arguments.Add(UndefinedValue.Default);
            }

            using (_dynamicScope.OpenScope())
            {
                for (int index = 0; index < body.FunctionDeclaration.Parameters.Count; index++)
                {
                    _dynamicScope.Register(body.FunctionDeclaration.Parameters[index], arguments[index]);
                }
                return body.FunctionDeclaration.Body.Accept(this);
            }
        }
    }
}
