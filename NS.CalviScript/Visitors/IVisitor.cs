namespace NS.CalviScript
{
    public interface IVisitor<T>
    {
        T Visit(BlockExpression expression);

        T Visit(ConstantExpression expression);

        T Visit(LookUpExpression expression);

        T Visit(UnaryExpression expression);

        T Visit(BinaryExpression expression);

        T Visit(TernaryExpression expression);

        T Visit(VariableDeclarationExpression expression);

        T Visit(ErrorExpression expression);

        T Visit(AssignExpression expression);

        T Visit(UndefinedExpression expression);
    }

    public static class IVisitorExtentions
    {
        public static T Visit<T>(this IVisitor<T> @this, IExpression expression)
            => expression.Accept(@this);
    }
}
