using System;

namespace NS.CalviScript
{
    public class NumberExpression : IExpression
    {
        public NumberExpression(int value)
        {
            Value = value;
        }
        public int Value { get; }

        public string ToLispyString() => Value.ToString();

        public string ToInfixString() => Value.ToString();
    }
}
