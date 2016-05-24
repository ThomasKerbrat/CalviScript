using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    class VisitorTests
    {
        [TestCase("5 + 10 % 2", "[+ 5 [% 10 2]]")]
        [TestCase("-5 + 10 % 2", "[+ [- 5] [% 10 2]]")]
        [TestCase("(8 + 10) * (11 % 15)", "[* [+ 8 10] [% 11 15]]")]
        public void should_stringify_to_lispy(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            IExpression expression = parser.ParseExpression();
            var visitor = new LispyStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("2 + 7 / 12", "(2 + (7 / 12))")]
        [TestCase("70 % (50 - 4 * 6)", "(70 % (50 - (4 * 6)))")]
        public void should_stringify_to_infix(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            IExpression expression = parser.ParseExpression();
            var visitor = new InfixStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("1", 1)]
        [TestCase("1 + 1", 2)]
        [TestCase("(3 + 7) / (5 - 2)", 3)]
        [TestCase("70 % (50 - 4 * 6)", 18)]
        public void should_evaluate(string input, int expected)
        {
            IExpression expression = Parser.Parse(input);
            EvaluationVisitor visitor = new EvaluationVisitor();

            int result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
