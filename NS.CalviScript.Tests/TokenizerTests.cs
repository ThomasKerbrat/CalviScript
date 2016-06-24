using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NS.CalviScript.Tests
{
    [TestFixture]
    public class TokenizerTests
    {
        [TestCase(" ", TokenType.End)]
        [TestCase("+", TokenType.Plus)]
        [TestCase("-", TokenType.Minus)]
        [TestCase("*", TokenType.Mult)]
        [TestCase("/", TokenType.Div)]
        [TestCase("%", TokenType.Modulo)]
        [TestCase("(", TokenType.LeftParenthesis)]
        [TestCase(")", TokenType.RightParenthesis)]
        [TestCase("?", TokenType.QuestionMark)]
        [TestCase(":", TokenType.Colon)]
        [TestCase("{", TokenType.OpenCurly)]
        [TestCase("}", TokenType.CloseCurly)]
        [TestCase(",", TokenType.Coma)]
        public void should_parse_string_containing_one_char(string input, TokenType expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Token token = tokenizer.GetNextToken();
            Assert.That(token.Type, Is.EqualTo(expected));
        }

        [TestCase("(", TokenType.LeftParenthesis, TokenType.End)]
        [TestCase("()", TokenType.LeftParenthesis, TokenType.RightParenthesis)]
        [TestCase("( )", TokenType.LeftParenthesis, TokenType.RightParenthesis)]
        [TestCase("11", TokenType.Number, TokenType.End)]
        [TestCase("1 1", TokenType.Number, TokenType.Number)]
        public void should_parse_two_tokens(string input, TokenType type1, TokenType type2)
        {
            Tokenizer tokenizer = new Tokenizer(input);

            Token t1 = tokenizer.GetNextToken();
            Token t2 = tokenizer.GetNextToken();

            Assert.That(t1.Type, Is.EqualTo(type1));
            Assert.That(t2.Type, Is.EqualTo(type2));
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

        [TestCase("0")]
        [TestCase("1")]
        [TestCase("123456789")]
        [TestCase("999")]
        public void should_parse_numbers(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            var token = tokenizer.GetNextToken();
            Assert.That(token.Type, Is.EqualTo(TokenType.Number));
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

        [TestCase("/*", TokenType.Error)]
        [TestCase("*/", TokenType.Mult, TokenType.Div)]
        [TestCase("/**/", TokenType.End)]
        [TestCase("1 /* 2 + 2 */ /* var x */ x", TokenType.Number, TokenType.Identifier, TokenType.End)]
        [TestCase("1 /* \r\n\r\n */ /* \r\n */ x", TokenType.Number, TokenType.Identifier, TokenType.End)]
        [TestCase("1 /* 2 + 2 */ /* var x */ x /* */", TokenType.Number, TokenType.Identifier, TokenType.End)]
        public void should_parse_multiline_comments(string input, params TokenType[] expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            List<TokenType> expectedTokens = new List<TokenType>(expected);

            foreach (TokenType token in expectedTokens)
            {
                tokenizer.GetNextToken();
                Assert.That(tokenizer.CurrentToken.Type, Is.EqualTo(token));
            }
        }

        [TestCase("/*/**/*/", TokenType.End)]
        [TestCase("/* /* */", TokenType.Error)]
        [TestCase("1 /* 2 /* 3 */ 4 */ z", TokenType.Number, TokenType.Identifier, TokenType.End)]
        public void should_parse_nested_multiline_comments(string input, params TokenType[] expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            List<TokenType> expectedTokens = new List<TokenType>(expected);

            foreach (TokenType token in expectedTokens)
            {
                tokenizer.GetNextToken();
                Assert.That(tokenizer.CurrentToken.Type, Is.EqualTo(token));
            }
        }

        [TestCase("_", TokenType.Identifier)]
        [TestCase("_0", TokenType.Identifier)]
        [TestCase("_0var", TokenType.Identifier)]
        [TestCase("_0name", TokenType.Identifier)]
        [TestCase("var", TokenType.Var)]
        [TestCase("VAR", TokenType.Identifier)]
        [TestCase("var0", TokenType.Identifier)]
        [TestCase("_var", TokenType.Identifier)]
        [TestCase("_Var", TokenType.Identifier)]
        [TestCase("_var0", TokenType.Identifier)]
        [TestCase("__var", TokenType.Identifier)]
        [TestCase("__Var", TokenType.Identifier)]
        [TestCase("__var0", TokenType.Identifier)]
        [TestCase("name", TokenType.Identifier)]
        [TestCase("NAME", TokenType.Identifier)]
        [TestCase("name0", TokenType.Identifier)]
        [TestCase("_name", TokenType.Identifier)]
        [TestCase("_Name", TokenType.Identifier)]
        [TestCase("_name0", TokenType.Identifier)]
        [TestCase("__name", TokenType.Identifier)]
        [TestCase("__Name", TokenType.Identifier)]
        [TestCase("__name0", TokenType.Identifier)]
        public void should_parse_identifiers(string input, TokenType expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            var type = tokenizer.GetNextToken();
            Assert.That(type.Type, Is.EqualTo(expected));
        }

        [TestCase("01", TokenType.Error, "01")]
        [TestCase("0name", TokenType.Error, "0n")]
        [TestCase("1name", TokenType.Error, "1n")]
        [TestCase("123name", TokenType.Error, "123n")]
        public void should_not_match_numbers(string input, TokenType expectedType, string expectedValue)
        {
            var tokenizer = new Tokenizer(input);

            tokenizer.GetNextToken();

            Assert.That(tokenizer.CurrentToken.Type, Is.EqualTo(expectedType));
            Assert.That(tokenizer.CurrentToken.Value, Is.EqualTo(expectedValue));
        }

        [TestCase("var", TokenType.Var, "var")]
        [TestCase("while", TokenType.While, "while")]
        [TestCase("function", TokenType.Function, "function")]
        public void should_parse_special_identifiers(string input, TokenType expectedType, string expectedValue)
        {
            var tokenizer = new Tokenizer(input);

            tokenizer.GetNextToken();

            Assert.That(tokenizer.CurrentToken.Type, Is.EqualTo(expectedType));
            Assert.That(tokenizer.CurrentToken.Value, Is.EqualTo(expectedValue));
        }
    }
}
