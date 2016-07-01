using NUnit.Framework;
using System.Collections.Generic;

namespace NS.CalviScript.Tests.Visitors
{
    [TestFixture]
    public class EvaluationVisitorTests
    {
        [TestCase("1", 1)]
        [TestCase("1 + 1", 2)]
        [TestCase("(3 + 7) / (5 - 2)", 3)]
        [TestCase("70 % (50 - 4 * 6)", 18)]
        [TestCase("1 ? 2 : 3", 2)]
        [TestCase("-1 ? 2 : 3", 3)]
        public void should_evaluate_expression(string input, int expected)
        {
            IExpression expression = Parser.ParseExpression(input);
            var globalContext = new Dictionary<string, BaseValue>();
            EvaluationVisitor visitor = new EvaluationVisitor(globalContext);

            var result = visitor.Visit(expression);

            Assert.That(result, Is.InstanceOf<IntegerValue>());
            Assert.That(((IntegerValue)result).Value, Is.EqualTo(expected));
        }

        [TestCase("x;", 3712)]
        [TestCase("x + 10;", 3712 + 10)]
        [TestCase("(x * x) + 10;", 3712 * 3712 + 10)]
        [TestCase("var a = 3;", 3)]
        [TestCase("var x; var a = 3; var b = 1; { var a = 7; x = a + b; } x;", 8)]
        [TestCase("var a = 3; var collector = 0; while (a) { a = a - 1; collector = collector + 10; }", 40)]
        [TestCase("var a = 3; var collector = 0; while (a) { a = a - 1; collector = collector + 10; } collector;", 40)]
        public void should_evaluate_program_with_global_context(string program, int expected)
        {
            IExpression expression = Parser.ParseProgram(program);
            var globalContext = new Dictionary<string, BaseValue>();
            globalContext.Add("x", IntegerValue.Create(3712));
            EvaluationVisitor visitor = new EvaluationVisitor(globalContext);

            BaseValue result = visitor.Visit(expression);

            Assert.That(result, Is.InstanceOf<IntegerValue>());
            Assert.That(((IntegerValue)result).Value, Is.EqualTo(expected));
        }

        [TestCase("")]
        // [TestCase(";")]
        public void should_evaluate_to_undefined(string input)
        {
            IExpression expression = Parser.ParseProgram(input);
            EvaluationVisitor visitor = new EvaluationVisitor(new Dictionary<string, BaseValue>());

            BaseValue result = visitor.Visit(expression);

            Assert.That(result, Is.InstanceOf<UndefinedValue>());
        }

        [TestCase("var count = 21 var i = 5 while (i) { i = i - 1 var count = 42 } count", 21)]
        [TestCase("var count = 21 function () { var count = 42 } count", 21)]
        [TestCase("var f = function (a) { a + 10 }; f(3);", 13)]
        [TestCase("var x = function (b) { var a = 3; while (b - 1) { a = a + 1; var a = a + b; b = b - 1; } }; x(2);", 5)]
        [TestCase("var add10 = function (a) { a + 10; }; var add15 = function (a) { add10(a + 5); }; add10(add15(100));", 125)]
        [TestCase("var recurse; recurse = function (a) { a ? a + recurse(a - 1) : 0; }; recurse(3)", 6)]
        public void should_open_syntaxic_scope(string program, int expected)
        {
            IExpression expression = Parser.ParseProgram(program);
            var globalContext = new Dictionary<string, BaseValue>();
            EvaluationVisitor visitor = new EvaluationVisitor(globalContext);

            BaseValue result = visitor.Visit(expression);

            Assert.That(result, Is.InstanceOf<IntegerValue>());
            Assert.That(((IntegerValue)result).Value, Is.EqualTo(expected));

        }
    }
}
