namespace NS.CalviScript
{
    public interface IIdentifierExpression : IExpression
    {
        /// <summary>
        /// Gets the name of the indentifier.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the associated <see cref="VariableDeclaration"/>
        /// to which this identihier is statically bound.
        /// </summary>
        VariableDeclarationExpression VariableDeclaration { get; }
    }
}
