using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    class VisitorTests
    {
        [TestCase("5 + 10 % 2", "[+ 5 [% 10 2]]")]
        [TestCase("-5 + 10 % 2", "[+ [- 5] [% 10 2]]")]
        [TestCase("(8 + 10) * (11 % 15)", "[* [+ 8 10] [% 11 15]]")]
        [TestCase("1 ? 2 : 3", "[? 1 2 3]")]
        public void should_stringify_to_lispy(string input, string expected)
        {
            IExpression expression = Parser.ParseExpression(input);
            var visitor = new LispyStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("2 + 7 / 12", "(2 + (7 / 12))")]
        [TestCase("70 % (50 - 4 * 6)", "(70 % (50 - (4 * 6)))")]
        [TestCase("1 ? 2 : 3", "(1 ? 2 : 3)")]
        [TestCase("12 * (-2 + 5) ? (1 ? 5 * 5 : 0) : (-5 * 5 ? 8 : 8 % 3)", "((12 * (-2 + 5)) ? (1 ? (5 * 5) : 0) : ((-5 * 5) ? 8 : (8 % 3)))")]
        public void should_stringify_to_infix(string input, string expected)
        {
            var visitor = new InfixStringVisitor();

            IExpression expression = Parser.ParseExpression(input);

            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1", 1)]
        [TestCase("1 + 1", 2)]
        [TestCase("(3 + 7) / (5 - 2)", 3)]
        [TestCase("70 % (50 - 4 * 6)", 18)]
        [TestCase("1 ? 2 : 3", 2)]
        [TestCase("-1 ? 2 : 3", 3)]
        public void should_evaluate(string input, int expected)
        {
            IExpression expression = Parser.ParseExpression(input);
            EvaluationVisitor visitor = new EvaluationVisitor();

            int result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
