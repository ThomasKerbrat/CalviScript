﻿using System;

namespace NS.CalviScript
{
    public class EvaluationVisitor : IVisitor<int>
    {
        public int Visit(BlockExpression expression)
        {
            throw new NotImplementedException();
        }

        public int Visit(ConstantExpression expression)
            => expression.Value;

        int IVisitor<int>.Visit(LookUpExpression expression)
        {
            throw new NotImplementedException();
        }

        public int Visit(UnaryExpression expression)
        {
            switch (expression.OperatorType)
            {
                case TokenType.Minus:
                    return -1 * expression.Expression.Accept(this);
                default:
                    throw new ArgumentException("Not an operator.");
            }
        }

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

        public int Visit(TernaryExpression expression)
        {
            return expression.PredicateExpression.Accept(this) >= 0 ?
                expression.TrueExpression.Accept(this) :
                expression.FalseExpression.Accept(this);
        }

        public int Visit(VariableDeclarationExpression expression)
        {
            throw new NotImplementedException();
        }

        public int Visit(ErrorExpression expression)
        {
            throw new InvalidOperationException(expression.Message);
        }
    }
}
