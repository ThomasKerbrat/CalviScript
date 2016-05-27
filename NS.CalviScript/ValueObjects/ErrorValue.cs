namespace NS.CalviScript
{
    public class ErrorValue : BaseValue
    {
        public ErrorValue(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public override string ToString()
            => "Error: " + Message;
    }
}
