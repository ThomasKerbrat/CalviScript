using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase("2 * 3 + 5", "[+ [* 2 3] 5]")]
        [TestCase("(11 + 7) / 15 % 8", "[% [/ [+ 11 7] 15] 8]")]
        [TestCase("10 + (11 + 50)","[+ 10 [+ 11 50]]")]
        [TestCase("2 + 3 * 5", "[+ 2 [* 3 5]]")]
        [TestCase("2 * -7", "[* 2 [- 7]]")]
        public void should_parse_some_complex_operation(string input, string expected)
        {
            var visitor = new LispyStringVisitor();

            IExpression expression = Parser.Parse(input);

            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [Test]
        public void should_stringify()
        {
            var visitor = new InfixStringVisitor();
            IExpression expression = new BinaryExpression(
                TokenType.Plus,
                new BinaryExpression(
                    TokenType.Plus,
                    new ConstantExpression(2),
                    new ConstantExpression(7)),
                new BinaryExpression(
                    TokenType.Mult,
                    new ConstantExpression(5),
                    new ConstantExpression(8)));

            Assert.That(visitor.Visit(expression), Is.EqualTo("((2 + 7) + (-5 * 8))"));
        }
    }
}
