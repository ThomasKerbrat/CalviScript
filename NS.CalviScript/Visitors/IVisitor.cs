namespace NS.CalviScript
{
    public interface IVisitor
    {
        void VisitBinaryExpression(BinaryExpression expression);

        void VisitConstantExpression(ConstantExpression expression);

        void VisitErrorExpression(ErrorExpression expression);
    }

    public static class IVisitorExtentions
    {
        public static void Visit(this IVisitor @this, IExpression expression)
            => expression.Accept(@this);
    }
}
