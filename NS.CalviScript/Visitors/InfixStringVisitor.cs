using System;

namespace NS.CalviScript
{
    public class InfixStringVisitor : IVisitor<string>
    {
        public string Visit(ProgramExpression expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(ConstantExpression expression)
            => expression.Value.ToString();

        public string Visit(LookUpExpression expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(UnaryExpression expression)
            => string.Format("{0}{1}",
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                expression.Expression.Accept(this));

        public string Visit(BinaryExpression expression)
            => string.Format("({0} {1} {2})",
                expression.LeftExpression.Accept(this),
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                expression.RightExpression.Accept(this));

        public string Visit(TernaryExpression expression)
            => string.Format("({0} ? {1} : {2})",
                expression.PredicateExpression.Accept(this),
                expression.TrueExpression.Accept(this),
                expression.FalseExpression.Accept(this));

        public string Visit(VariableDeclarationExpression expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(ErrorExpression expression)
            => string.Format("[Error {0}]", expression.Message);
    }
}
