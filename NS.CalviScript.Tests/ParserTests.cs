using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase("2 * 3 + 5", "[+ [* 2 3] 5]")]
        [TestCase("(11 + 7) / 15 % 8", "[% [/ [+ 11 7] 15] 8]")]
        [TestCase("10 + (11 + 50)", "[+ 10 [+ 11 50]]")]
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

        [TestCase("function () {}", "[S [function [] [S]]]")]
        [TestCase("function (a) {}", @"[S [function [[VD ""a""]] [S]]]")]
        [TestCase("function (a,) {}", @"[S [function [[VD ""a""]] [S]]]")]
        [TestCase("function (a, b) {}", @"[S [function [[VD ""a""] [VD ""b""]] [S]]]")]
        [TestCase("function (a) { a; }", @"[S [function [[VD ""a""]] [S [LU ""a""]]]]")]
        [TestCase("function (a) { function (b) { a + b; } }", @"[S [function [[VD ""a""]] [S [function [[VD ""b""]] [S [+ [LU ""a""] [LU ""b""]]]]]]]")]
        [TestCase("function (a) { return a; }", @"[S [function [[VD ""a""]] [S [return [LU ""a""]]]]]")]
        public void should_parse_function_delcaration_expression(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseProgram(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("do()", @"[S [FC [LU ""do""] []]]")]
        [TestCase("do(a)", @"[S [FC [LU ""do""] [[LU ""a""]]]]")]
        [TestCase("do(a, b)", @"[S [FC [LU ""do""] [[LU ""a""] [LU ""b""]]]]")]
        [TestCase("do(a, b,)", @"[S [FC [LU ""do""] [[LU ""a""] [LU ""b""]]]]")]
        [TestCase("do(1 + 1)", @"[S [FC [LU ""do""] [[+ 1 1]]]]")]
        [TestCase("do(do2(1 + 1))", @"[S [FC [LU ""do""] [[FC [LU ""do2""] [[+ 1 1]]]]]]")]
        [TestCase("do(function () { 1 + 1; })", @"[S [FC [LU ""do""] [[function [] [S [+ 1 1]]]]]]")]
        public void should_parse_function_call_expression(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseProgram(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1;", "[S 1]")]
        [TestCase("1 ? 5 % 3 : (1 + 1);", "[S [? 1 [% 5 3] [+ 1 1]]]")]
        [TestCase("var test = 3 + 50; var test2 = test * 3;", @"[S [= [VD ""test""] [+ 3 50]] [= [VD ""test2""] [* [LU ""test""] 3]]]")]
        public void should_parse_program(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            IExpression expression = Parser.ParseProgram(input);
            Assert.That(visitor.Visit(expression), Is.EqualTo(expected));
        }

        [TestCase("1", "[S 1]")]
        [TestCase("1 + 1", "[S [+ 1 1]]")]
        [TestCase("var a = 1 \r\n var b = 2 \r\n a + b", @"[S [= [VD ""a""] 1] [= [VD ""b""] 2] [+ [LU ""a""] [LU ""b""]]]")]
        public void should_parse_program_with_no_semicolons(string input, string expected)
        {
            IExpression expression = Parser.ParseProgram(input);
            LispyStringVisitor visitor = new LispyStringVisitor();

            string result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
