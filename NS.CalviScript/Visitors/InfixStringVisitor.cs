using System.Text;

namespace NS.CalviScript
{
    public class InfixStringVisitor : IVisitor<string>
    {
        public string Visit(BlockExpression expression)
        {
            var sb = new StringBuilder();
            sb.Append("{");

            foreach (var statement in expression.Statements)
            {
                sb.Append(" ");
                sb.Append(statement.Accept(this));
                sb.Append(";");
            }

            if (sb.Length > 1)
                sb.Append(" ");

            sb.Append("}");
            return sb.ToString();
        }

        public string Visit(ConstantExpression expression)
            => expression.Value.ToString();

        public string Visit(LookUpExpression expression)
            => expression.Identifier;

        public string Visit(AssignExpression expression)
            => string.Format("{0} = {1}",
                expression.Identifier.Accept(this),
                expression.Expression.Accept(this));

        public string Visit(UndefinedExpression expression)
            => "undefined";

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
            => string.Format("var {0}",
                expression.Identifier);

        public string Visit(ErrorExpression expression)
            => string.Format("[Error {0}]", expression.Message);

        public string Visit(WhileExpression expression)
            => string.Format("while ({0}) {1}",
                expression.Condition.Accept(this),
                expression.Body.Accept(this));
    }
}
