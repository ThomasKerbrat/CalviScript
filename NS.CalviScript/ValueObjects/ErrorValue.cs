using System;

namespace NS.CalviScript
{
    public class ErrorValue : BaseValue
    {
        public ErrorValue(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public override bool IsTrue
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
            => "Error: " + Message;
    }
}
