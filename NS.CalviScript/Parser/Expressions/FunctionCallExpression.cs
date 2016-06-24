using System.Collections.Generic;

namespace NS.CalviScript
{
    public class FunctionCallExpression : IExpression
    {
        public FunctionCallExpression(LookUpExpression name, IReadOnlyList<IExpression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public LookUpExpression Name { get; }

        public IReadOnlyList<IExpression> Arguments { get; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
