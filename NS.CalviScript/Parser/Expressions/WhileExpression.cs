namespace NS.CalviScript
{
    public class WhileExpression : IExpression
    {
        public WhileExpression(IExpression condition, BlockExpression body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; private set; }

        public BlockExpression Body { get; private set; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
