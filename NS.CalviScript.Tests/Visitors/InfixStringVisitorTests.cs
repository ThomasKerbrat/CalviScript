using NUnit.Framework;

namespace NS.CalviScript.Tests.Visitors
{
    [TestFixture]
    public class InfixStringVisitorTests
    {
        void boilerplate(string input, string expected)
        {
            var visitor = new InfixStringVisitor();
            var expression = Parser.ParseExpression(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("", "")]
        [TestCase("1;", "1;")]
        [TestCase("1 + 1;", "(1 + 1);")]
        [TestCase("var test = 1 + 1;", "var test = (1 + 1);")]
        [TestCase("var test = 1 + 1; var test2 = test - 8;", "var test = (1 + 1); var test2 = (test - 8);")]
        public void should_visit_program_ast(string input, string expected)
        {
            var visitor = new InfixStringVisitor();
            var expression = Parser.ParseProgram(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("0", "0")]
        [TestCase("5", "5")]
        [TestCase("99", "99")]
        public void should_visit_constant_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("-0", "-0")]
        [TestCase("-5", "-5")]
        [TestCase("-99", "-99")]
        public void should_visit_unary_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("0 + 1", "(0 + 1)")]
        [TestCase("5 / 5", "(5 / 5)")]
        [TestCase("99 * -2", "(99 * -2)")]
        public void should_visit_binary_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("0 ? 1 : 2", "(0 ? 1 : 2)")]
        [TestCase("5 ? 5 + 5 : -5", "(5 ? (5 + 5) : -5)")]
        [TestCase("-99 ? 9 : -99", "(-99 ? 9 : -99)")]
        public void should_visit_ternary_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("a = 1", "a = 1")]
        [TestCase("a = 1 + 1", "a = (1 + 1)")]
        [TestCase("a = b + -9", "a = (b + -9)")]
        public void should_visit_assignation_ast(string input, string expected)
            => boilerplate(input, expected);
    }
}
