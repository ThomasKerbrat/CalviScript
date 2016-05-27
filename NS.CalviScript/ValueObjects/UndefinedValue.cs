namespace NS.CalviScript
{
    public class UndefinedValue : BaseValue
    {
        public static UndefinedValue Default = new UndefinedValue();

        private UndefinedValue() { }

        public override string ToString()
            => "undefined";
    }
}
