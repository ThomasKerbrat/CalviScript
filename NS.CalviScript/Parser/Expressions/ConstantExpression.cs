namespace NS.CalviScript
{
    /// <summary>
    /// This class should be named "LiteralExpression".
    /// </summary>
    public class ConstantExpression : IExpression
    {
        public ConstantExpression(int value)
        {
            Value = value;
        }

        public int Value { get; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
