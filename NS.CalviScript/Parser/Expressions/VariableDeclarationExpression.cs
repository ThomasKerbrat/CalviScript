using System;

namespace NS.CalviScript
{
    public class VariableDeclarationExpression : IExpression, IIdentifierExpression
    {
        public VariableDeclarationExpression(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        VariableDeclarationExpression IIdentifierExpression.VariableDeclaration
            => this;

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
