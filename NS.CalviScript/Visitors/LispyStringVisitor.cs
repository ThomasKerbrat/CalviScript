using System;
using System.Text;

namespace NS.CalviScript
{
    public class LispyStringVisitor : IVisitor<string>
    {
        public string Visit(BlockExpression expression)
        {
            var sb = new StringBuilder();
            sb.Append("[S");

            foreach (var expression_i in expression.Statements)
            {
                sb.Append(" ");
                sb.Append(expression_i.Accept(this));
            }

            sb.Append(']');
            return sb.ToString();
        }

        public string Visit(ConstantExpression expression)
            => expression.Value.ToString();

        public string Visit(LookUpExpression expression)
            => string.Format("[LU \"{0}\"]", expression.Identifier);

        public string Visit(AssignExpression expression)
            => string.Format(@"[= {0} {1}]",
                expression.Identifier.Accept(this),
                expression.Expression.Accept(this));

        public string Visit(UndefinedExpression expression)
            => "undefined";

        public string Visit(UnaryExpression expression)
            => string.Format("[{0} {1}]",
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                expression.Expression.Accept(this));

        public string Visit(BinaryExpression expression)
            => string.Format("[{0} {1} {2}]",
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                expression.LeftExpression.Accept(this),
                expression.RightExpression.Accept(this));

        public string Visit(TernaryExpression expression)
            => string.Format("[? {0} {1} {2}]",
                expression.PredicateExpression.Accept(this),
                expression.TrueExpression.Accept(this),
                expression.FalseExpression.Accept(this));

        public string Visit(VariableDeclarationExpression expression)
            => string.Format("[VD \"{0}\"]",
                expression.Identifier);

        public string Visit(ErrorExpression expression)
            => string.Format("[Error {0}]",
                expression.Message);

        public string Visit(WhileExpression expression)
            => string.Format("[while {0} {1}]",
                expression.Condition.Accept(this),
                expression.Body.Accept(this));

        public string Visit(FunctionDeclarationExpression expression)
        {
            var sb = new StringBuilder("[");

            var fisrt = true;
            foreach (var param in expression.Parameters)
            {
                if (fisrt) fisrt = false;
                else sb.Append(" ");
                sb.Append(param.Accept(this));
            }
            sb.Append("]");

            return string.Format("[function {0} {1}]", sb.ToString(), expression.Body.Accept(this));
        }
    }
}
