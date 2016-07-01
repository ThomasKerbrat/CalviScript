namespace NS.CalviScript
{
    public class ErrorExpression : IExpression
    {
        public ErrorExpression(string message)
        {
            Message = message;
        }

        public string Message { get; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
