using System;

namespace NS.CalviScript
{
    public class ConstantExpression : IExpression
    {
        public ConstantExpression(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public string ToLispyString() => Value.ToString();

        public string ToInfixString() => Value.ToString();

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
