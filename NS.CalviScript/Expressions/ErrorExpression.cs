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
    }
}
