﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.CalviScript.Tests.Visitors
{
    [TestFixture]
    public class EvaluationVisitorTests
    {
        /// <summary>
        /// var x;
        /// var a = 3;
        /// var b = 1;
        /// {
        ///     var a = 7;
        ///     x = a + b;
        /// }
        /// x;
        /// => 8
        /// </summary>
        /// <param name="program"></param>
        /// <param name="expected"></param>
        [TestCase("x;", 3712)]
        [TestCase("x + 10;", 3712 + 10)]
        [TestCase("(x * x) + 10;", 3712 * 3712 + 10)]
        [TestCase("var a = 3;", 3)]
        public void should_access_to_the_context(string program, int expected)
        {
            IExpression expression = Parser.ParseProgram(program);
            var globalContext = new Dictionary<string, BaseValue>();
            globalContext.Add("x", IntegerValue.Create(3712));
            EvaluationVisitor visitor = new EvaluationVisitor(globalContext);

            BaseValue result = visitor.Visit(expression);

            Assert.That(result, Is.InstanceOf<IntegerValue>());
            Assert.That(((IntegerValue)result).Value, Is.EqualTo(expected));
        }
    }
}
