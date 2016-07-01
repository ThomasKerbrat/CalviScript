using System;

namespace NS.CalviScript
{
    public class TernaryExpression : IExpression
    {
        public TernaryExpression(IExpression predicate, IExpression @true, IExpression @false)
        {
            PredicateExpression = predicate;
            TrueExpression = @true;
            FalseExpression = @false;
        }

        public IExpression PredicateExpression;
        public IExpression FalseExpression;
        public IExpression TrueExpression;

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
