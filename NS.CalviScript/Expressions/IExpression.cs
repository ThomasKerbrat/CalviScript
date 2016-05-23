namespace NS.CalviScript
{
    public interface IExpression
    {
        string ToLispyString();

        string ToInfixString();

        void Accept(IVisitor visitor);
    }
}
