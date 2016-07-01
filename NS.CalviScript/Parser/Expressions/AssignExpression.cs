namespace NS.CalviScript
{
    public class AssignExpression : IExpression
    {
        public AssignExpression(IIdentifierExpression identifier, IExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public IIdentifierExpression Identifier { get; }

        public IExpression Expression { get; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
