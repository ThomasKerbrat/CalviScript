using System;

namespace NS.CalviScript
{
    public class ErrorExpression : IExpression
    {
        public ErrorExpression(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public string ToLispyString() => string.Format("[Error {0}]", Message);

        public string ToInfixString() => string.Format("[Error {0}]", Message);

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
