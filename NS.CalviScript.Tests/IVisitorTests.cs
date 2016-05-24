using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    class IVisitorTests
    {
        [Test]
        public void should_stringify_to_lispy()
        {
            Tokenizer tokenizer = new Tokenizer("(8 + 10) * (11 % 15)");
            Parser parser = new Parser(tokenizer);
            IExpression expression = parser.ParseExpression();
            LispyStringVisitor visitor = new LispyStringVisitor();

            visitor.Visit(expression);

            Assert.That(visitor.Result, Is.EqualTo("[* [+ 8 10] [% 11 15]]"));
        }

        [Test]
        public void should_stringify_to_infix()
        {
            Tokenizer tokenizer = new Tokenizer("2 + 7 / 12");
            Parser parser = new Parser(tokenizer);
            IExpression expression = parser.ParseExpression();
            InfixStringVisitor visitor = new InfixStringVisitor();

            visitor.Visit(expression);

            Assert.That(visitor.Result, Is.EqualTo("(2 + (7 / 12))"));
        }

        [TestCase("1", 1)]
        [TestCase("1 + 1", 2)]
        [TestCase("(3 + 7) / (5 - 2)", 3)]
        public void should_evaluate(string input, int expected)
        {
            Parser parser = new Parser(new Tokenizer(input));
            IExpression expression = parser.ParseExpression();
            EvaluationVisitor visitor = new EvaluationVisitor();

            visitor.Visit(expression);

            Assert.That(visitor.Result, Is.EqualTo(expected));
        }
    }
}
