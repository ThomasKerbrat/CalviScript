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

        [Test]
        public void should_stringify_to_lispy_using_generic_visitor()
        {
            Tokenizer tokenizer = new Tokenizer("5 + 10 % 2");
            Parser parser = new Parser(tokenizer);
            IExpression expression = parser.ParseExpression();
            GenericLispyStringVisitor visitor = new GenericLispyStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo("[+ 5 [% 10 2]]"));
        }

        [Test]
        public void should_stringify_to_infix_using_generic_visitor()
        {
            IExpression expression = Parser.Parse("70 % (50 - 4 * 6)");
            GenericInfixStringVisitor visitor = new GenericInfixStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo("(70 % (50 - (4 * 6)))"));
        }

        [Test]
        public void should_evaluate_using_generic_visitor()
        {
            IExpression expression = Parser.Parse("70 % (50 - 4 * 6)");
            GenericEvaluationVisitor visitor = new GenericEvaluationVisitor();

            int result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(18));
        }
    }
}
