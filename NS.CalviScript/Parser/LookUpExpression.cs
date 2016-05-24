namespace NS.CalviScript
{
    public class LookUpExpression : IExpression
    {
        public LookUpExpression(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
