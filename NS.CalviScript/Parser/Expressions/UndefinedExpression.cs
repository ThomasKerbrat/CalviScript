namespace NS.CalviScript
{
    public class UndefinedExpression : IExpression
    {
        public static UndefinedExpression Default = new UndefinedExpression();

        UndefinedExpression() { }

        public int Value { get; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
