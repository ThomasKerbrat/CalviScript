namespace NS.CalviScript
{
    public class ConstantExpression : IExpression
    {
        public ConstantExpression(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
