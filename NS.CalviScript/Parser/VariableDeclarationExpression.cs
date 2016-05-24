using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.CalviScript
{
    public class VariableDeclarationExpression : IExpression
    {
        public VariableDeclarationExpression(string identifier, IExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public string Identifier { get; }

        public IExpression Expression { get; set; }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
