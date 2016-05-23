using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    class IExpressionTests
    {
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
