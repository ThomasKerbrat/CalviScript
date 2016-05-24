namespace NS.CalviScript
{
    public class LispyStringVisitor : IVisitor<string>
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
