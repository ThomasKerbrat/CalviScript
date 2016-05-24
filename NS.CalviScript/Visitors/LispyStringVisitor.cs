namespace NS.CalviScript
{
    public class LispyStringVisitor : IVisitor
    {
        public string Result { get; private set; }

        public void Visit(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            string left = Result;
            expression.RightExpression.Accept(this);
            string right = Result;

            Result = string.Format("[{0} {1} {2}]",
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                left,
                right);
        }

        public void Visit(ConstantExpression expression)
            => Result = expression.Value.ToString();

        public void Visit(ErrorExpression expression)
            => Result = string.Format("[Error {0}]", expression.Message);
    }

    public class GenericLispyStringVisitor : IVisitor<string>
    {
        public string Visit(ErrorExpression expression)
            => string.Format("[Error {0}]", expression.Message);

        public string Visit(ConstantExpression expression)
            => expression.Value.ToString();

        public string Visit(BinaryExpression expression)
            => string.Format("[{0} {1} {2}]", 
                TokenTypeHelpers.TokenTypeToString(expression.OperatorType),
                expression.LeftExpression.Accept(this),
                expression.RightExpression.Accept(this));
    }
}
