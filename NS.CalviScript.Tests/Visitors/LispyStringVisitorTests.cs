using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    public class LispyStringVisitorTests
    {
        void boilerplate(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseExpression(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("", "[S]")]
        [TestCase("1;", "[S 1]")]
        [TestCase("1 + 1;", "[S [+ 1 1]]")]
        [TestCase("var test = 1 + 1;", @"[S [VD ""test"" [+ 1 1]]]")]
        [TestCase("var test = 1 + 1; var test2 = test - 8;", @"[S [VD ""test"" [+ 1 1]] [VD ""test2"" [- [LU ""test""] 8]]]")]
        public void should_visit_program_ast(string input, string expected)
        {
            var visitor = new LispyStringVisitor();
            var expression = Parser.ParseProgram(input);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("0", "0")]
        [TestCase("5", "5")]
        [TestCase("99", "99")]
        public void should_visit_constant_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("-0", "[- 0]")]
        [TestCase("-5", "[- 5]")]
        [TestCase("-99", "[- 99]")]
        public void should_visit_unary_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("0 + 1", "[+ 0 1]")]
        [TestCase("5 / 5", "[/ 5 5]")]
        [TestCase("99 * -2", "[* 99 [- 2]]")]
        public void should_visit_binary_ast(string input, string expected)
            => boilerplate(input, expected);

        [TestCase("0 ? 1 : 2", "[? 0 1 2]")]
        [TestCase("5 ? 5 + 5 : -5", "[? 5 [+ 5 5] [- 5]]")]
        [TestCase("-99 ? 9 : -99", "[? [- 99] 9 [- 99]]")]
        public void should_visit_ternary_ast(string input, string expected)
            => boilerplate(input, expected);
    }
}
