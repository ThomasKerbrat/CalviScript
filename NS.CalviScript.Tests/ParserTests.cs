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
        [TestCase("5 + 7 ? -8 * 2 : 7 * 3 ? 0 : 15", "[? [+ 5 7] [* [- 8] 2] [? [* 7 3] 0 15]]")]
        public void should_parse_some_complex_operation(string input, string expected)
        {
            var visitor = new LispyStringVisitor();

            IExpression expression = Parser.ParseExpression(input);

            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("-0", "[- 0]")]
        [TestCase("-1", "[- 1]")]
        public void should_parse_unary_expressions(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseExpression(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1 + 1", "[+ 1 1]")]
        public void should_parse_binary_expressions(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseExpression(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1 ? 2 : 3", "[? 1 2 3]")]
        [TestCase("-1 ? 2 : 3", "[? [- 1] 2 3]")]
        public void should_parse_ternary_expressions(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseExpression(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1;", "[S 1]")]
        [TestCase("1 ? 5 % 3 : (1 + 1);", "[S [? 1 [% 5 3] [+ 1 1]]]")]
        [TestCase("var test = 3 + 50; var test2 = test * 3;", @"[S [= [VD ""test""] [+ 3 50]] [= [VD ""test2""] [* [LU ""test""] 3]]]")]
        public void should_parse_program(string input, string expected)
        {
            var tokenizer = new Tokenizer(input);
            var parser = new Parser(tokenizer);
            var visitor = new LispyStringVisitor();

            IExpression expression = parser.ParseProgram();

            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }
    }
}
