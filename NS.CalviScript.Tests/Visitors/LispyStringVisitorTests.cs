using NUnit.Framework;

namespace NS.CalviScript.Tests.Visitors
{
    [TestFixture]
    public class LispyStringVisitorTests
    {
        void expressionBoilerplate(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseExpression(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        void programBoilerplate(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseProgram(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("", "[S]")]
        [TestCase("1;", "[S 1]")]
        [TestCase("var test = 1 + 1; var test2 = test - 8;", @"[S [= [VD ""test""] [+ 1 1]] [= [VD ""test2""] [- [LU ""test""] 8]]]")]
        public void should_visit_program_ast(string input, string expected)
            => programBoilerplate(input, expected);

        [TestCase("0", "0")]
        [TestCase("5", "5")]
        [TestCase("99", "99")]
        public void should_visit_constant_ast(string input, string expected)
            => expressionBoilerplate(input, expected);

        [TestCase("-0", "[- 0]")]
        [TestCase("-5", "[- 5]")]
        [TestCase("-99", "[- 99]")]
        public void should_visit_unary_ast(string input, string expected)
            => expressionBoilerplate(input, expected);

        [TestCase("0 + 1", "[+ 0 1]")]
        [TestCase("5 / 5", "[/ 5 5]")]
        [TestCase("99 * -2", "[* 99 [- 2]]")]
        public void should_visit_binary_ast(string input, string expected)
            => expressionBoilerplate(input, expected);

        [TestCase("0 ? 1 : 2", "[? 0 1 2]")]
        [TestCase("5 ? 5 + 5 : -5", "[? 5 [+ 5 5] [- 5]]")]
        [TestCase("-99 ? 9 : -99", "[? [- 99] 9 [- 99]]")]
        public void should_visit_ternary_ast(string input, string expected)
            => expressionBoilerplate(input, expected);

        [TestCase("a = 1", @"[= [LU ""a""] 1]")]
        [TestCase("a = 1 + 1", @"[= [LU ""a""] [+ 1 1]]")]
        [TestCase("a = b + -9", @"[= [LU ""a""] [+ [LU ""b""] [- 9]]]")]
        public void should_visit_assignation_ast(string input, string expected)
            => expressionBoilerplate(input, expected);

        [TestCase("var a = 1;", @"[S [= [VD ""a""] 1]]")]
        [TestCase("var a = 1 + 1;", @"[S [= [VD ""a""] [+ 1 1]]]")]
        [TestCase("var a = b + -9;", @"[S [= [VD ""a""] [+ [LU ""b""] [- 9]]]]")]
        public void should_visit_variableDeclaration_ast(string input, string expected)
            => programBoilerplate(input, expected);

        [TestCase("while (0) { 1 }", "[S [while 0 [S 1]]]")]
        [TestCase("var a = 3 while (a) { a = a - 1 }", @"[S [= [VD ""a""] 3] [while [LU ""a""] [S [= [LU ""a""] [- [LU ""a""] 1]]]]]")]
        public void should_visit_while_ast(string input, string expected)
            => programBoilerplate(input, expected);
    }
}
