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
                    new NumberExpression(2),
                    new NumberExpression(7)),
                new BinaryExpression(
                    TokenType.Mult,
                    new NumberExpression(5),
                    new NumberExpression(8)));


            Assert.That(expression.ToInfixString(), Is.EqualTo("((2 + 7) + (5 * 8))"));
        }
    }
}
