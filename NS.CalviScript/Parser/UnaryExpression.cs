namespace NS.CalviScript
{
    public class UnaryExpression : IExpression
    {
        public UnaryExpression(TokenType type, IExpression expression)
        {
            OperatorType = type;
            Expression = expression;
        }

        public TokenType OperatorType { get; private set; }

        public IExpression Expression { get; private set; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
