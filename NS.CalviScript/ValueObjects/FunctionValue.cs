namespace NS.CalviScript
{
    public class FunctionValue : BaseValue
    {
        public FunctionValue(FunctionDeclarationExpression expression)
        {
            FunctionDeclaration = expression;
        }

        public FunctionDeclarationExpression FunctionDeclaration { get; }

        public override bool IsTrue => true;
    }
}
