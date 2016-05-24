namespace NS.CalviScript
{
    public interface IVisitor<T>
    {
        T Visit(ConstantExpression expression);

        T Visit(UnaryExpression expression);

        T Visit(BinaryExpression expression);

        T Visit(TernaryExpression expression);

        T Visit(ErrorExpression expression);
    }

    public static class IVisitorExtentions
    {
        public static T Visit<T>(this IVisitor<T> @this, IExpression expression)
            => expression.Accept(@this);
    }
}
