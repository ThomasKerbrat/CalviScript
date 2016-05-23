namespace NS.CalviScript
{
    public interface IVisitor
    {
        void Visit(BinaryExpression expression);

        void Visit(ConstantExpression expression);

        void Visit(ErrorExpression expression);
    }

    public static class IVisitorExtentions
    {
        public static void Visit(this IVisitor @this, IExpression expression)
            => expression.Accept(@this);
    }
}
