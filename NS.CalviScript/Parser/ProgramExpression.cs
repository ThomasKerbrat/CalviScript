using System;
using System.Collections.Generic;

namespace NS.CalviScript
{
    public class ProgramExpression : IExpression
    {
        public ProgramExpression(IReadOnlyList<IExpression> statements)
        {
            Statements = statements;
        }

        public IReadOnlyList<IExpression> Statements { get; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
