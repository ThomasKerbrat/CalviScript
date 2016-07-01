using System.Collections.Generic;

namespace NS.CalviScript
{
    public class FunctionDeclarationExpression : IExpression
    {
        public FunctionDeclarationExpression(IReadOnlyList<VariableDeclarationExpression> parameters, BlockExpression body)
        {
            Parameters = parameters;
            Body = body;
        }

        public IReadOnlyList<VariableDeclarationExpression> Parameters { get; }

        public BlockExpression Body { get; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
