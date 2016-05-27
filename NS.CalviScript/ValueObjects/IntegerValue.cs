using System;

namespace NS.CalviScript
{
    public class IntegerValue : BaseValue
    {
        static IntegerValue M1 = new IntegerValue(-1);
        static IntegerValue Zero = new IntegerValue(0);
        static IntegerValue One = new IntegerValue(1);
        static IntegerValue Two = new IntegerValue(2);
        static IntegerValue Three = new IntegerValue(3);

        public static IntegerValue Create(int value)
        {
            switch (value)
            {
                case -1: return M1;
                case 0: return Zero;
                case 1: return One;
                case 2: return Two;
                case 3: return Three;
                default: return new IntegerValue(value);
            }
        }

        private IntegerValue(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override bool IsTrue
            => Value >= 0;

        public override string ToString()
            => Value.ToString();
    }
}
