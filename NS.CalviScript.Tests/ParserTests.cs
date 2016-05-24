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
        public void should_parse_some_complex_operation(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);

            IExpression expression = parser.ParseExpression();

            Assert.That(expression.ToLispyString(), Is.EqualTo(expected));
        }

        [Test]
        public void should_stringify()
        {
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


            Assert.That(expression.ToInfixString(), Is.EqualTo("((2 + 7) + (5 * 8))"));
        }
    }
}
