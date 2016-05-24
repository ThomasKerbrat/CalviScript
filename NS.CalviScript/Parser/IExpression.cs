namespace NS.CalviScript
{
    public interface IExpression
    {
        T Accept<T>(IVisitor<T> visitor);
    }
}
