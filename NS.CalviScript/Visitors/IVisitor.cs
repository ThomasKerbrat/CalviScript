namespace NS.CalviScript
{
    public interface IVisitor
    {
        void Visit(BinaryExpression expression);

        void Visit(ConstantExpression expression);

        void Visit(ErrorExpression expression);
    }

    public interface IVisitor<T>
    {
        T Visit(BinaryExpression expression);

        T Visit(ConstantExpression expression);

        T Visit(ErrorExpression expression);
    }

    public static class IVisitorExtentions
    {
        public static void Visit(this IVisitor @this, IExpression expression)
            => expression.Accept(@this);

        public static T Visit<T>(this IVisitor<T> @this, IExpression expression)
            => expression.Accept(@this);
    }
}
