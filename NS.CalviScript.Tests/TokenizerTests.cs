using NUnit.Framework;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    public class TokenizerTests
    {
        [TestCase("+", TokenType.Plus)]
        [TestCase("-", TokenType.Minus)]
        [TestCase("*", TokenType.Mult)]
        [TestCase("/", TokenType.Div)]
        [TestCase("%", TokenType.Modulo)]
        [TestCase("(", TokenType.LeftParenthesis)]
        [TestCase(")", TokenType.RightParenthesis)]
        public void should_parse_string_containing_one_char(string input, TokenType expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Token token = tokenizer.GetNextToken();
            Assert.That(token.Type, Is.EqualTo(expected));
        }

        [Test]
        public void should_parse_two_tokens()
        {
            Tokenizer tokenizer = new Tokenizer("+())");

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();
            Token t3 = tokenizer.GetNextToken();
            Token t4 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t2.Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(t3.Type, Is.EqualTo(TokenType.RightParenthesis));
            Assert.That(t4.Type, Is.EqualTo(TokenType.RightParenthesis));
        }

        [Test]
        public void should_parse_string_with_whitespace()
        {
            Tokenizer tokenizer = new Tokenizer("+\t (\r\n ) ");

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();
            Token t3 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t2.Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(t3.Type, Is.EqualTo(TokenType.RightParenthesis));
        }

        [Test]
        public void should_parse_numbers()
        {
            Tokenizer tokenizer = new Tokenizer("(10 + 59) + 12 )");

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();
            Token t3 = tokenizer.GetNextToken();
            Token t4 = tokenizer.GetNextToken();
            Token t5 = tokenizer.GetNextToken();
            Token t6 = tokenizer.GetNextToken();
            Token t7 = tokenizer.GetNextToken();
            Token t8 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(t2.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t2.Value, Is.EqualTo("10"));
            Assert.That(t3.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t4.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t4.Value, Is.EqualTo("59"));
            Assert.That(t5.Type, Is.EqualTo(TokenType.RightParenthesis));
            Assert.That(t6.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t7.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t7.Value, Is.EqualTo("12"));
            Assert.That(t8.Type, Is.EqualTo(TokenType.RightParenthesis));

        }

        [Test]
        public void should_parse_operator_number_and_parenthesis_symbols()
        {
            Tokenizer tokenizer = new Tokenizer("(10 + 59) + 12 )");

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();
            Token t3 = tokenizer.GetNextToken();
            Token t4 = tokenizer.GetNextToken();
            Token t5 = tokenizer.GetNextToken();
            Token t6 = tokenizer.GetNextToken();
            Token t7 = tokenizer.GetNextToken();
            Token t8 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(t2.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t2.Value, Is.EqualTo("10"));
            Assert.That(t3.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t4.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t4.Value, Is.EqualTo("59"));
            Assert.That(t5.Type, Is.EqualTo(TokenType.RightParenthesis));
            Assert.That(t6.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t7.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t7.Value, Is.EqualTo("12"));
            Assert.That(t8.Type, Is.EqualTo(TokenType.RightParenthesis));

        }

        [Test]
        public void should_parse_comments()
        {
            Tokenizer tokenizer = new Tokenizer("2 + // Comment\r\n5");

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();
            Token t3 = tokenizer.GetNextToken();
            Token t4 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t2.Type, Is.EqualTo(TokenType.Plus));
            Assert.That(t3.Type, Is.EqualTo(TokenType.Number));
            Assert.That(t4.Type, Is.EqualTo(TokenType.End));
        }
    }
}
