namespace NS.CalviScript
{
    public interface IIdentifierExpression : IExpression
    {
        string Identifier { get; }

        VariableDeclarationExpression VariableDeclaration { get; }
    }
}
