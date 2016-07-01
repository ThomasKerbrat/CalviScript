namespace NS.CalviScript
{
    public class LookUpExpression : IExpression, IIdentifierExpression
    {
        public LookUpExpression(string identifier, VariableDeclarationExpression varriableDeclaration)
        {
            Identifier = identifier;
            VariableDeclaration = varriableDeclaration;
        }

        public string Identifier { get; }

        public VariableDeclarationExpression VariableDeclaration { get; }

        [System.Diagnostics.DebuggerStepThrough]
        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
