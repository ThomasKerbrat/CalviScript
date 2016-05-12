using System;
using System.Diagnostics;
using System.Text;

namespace NS.CalviScript
{
    public class Tokenizer
    {
        readonly string _input;
        int _position;

        public Tokenizer(string input)
        {
            _input = input;
        }

        public bool IsEnd => _position + 1 >= _input.Length;

        public Token GetNextToken()
        {
            if (IsEnd) return new Token(TokenType.End);

            char current = Read();
            Token token;

            while (char.IsWhiteSpace(current )) current = Read();

            if (current == '+') token = new Token(TokenType.Plus);
            else if (current == '-') token = new Token(TokenType.Minus);
            else if (current == '*') token = new Token(TokenType.Star);
            else if (current == '/') token = new Token(TokenType.Slash);
            else if (current == '%') token = new Token(TokenType.Percent);
            else if (current == '(') token = new Token(TokenType.LeftParenthesis);
            else if (current == ')') token = new Token(TokenType.RightParenthesis);
            else if (char.IsDigit(current))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(current);
                while (!IsEnd && char.IsDigit(Peek())) sb.Append(Read());
                token = new Token(TokenType.Number, sb.ToString());
            }
            else token = new Token(TokenType.Error);

            return token;
        }

        char Read() => _input[_position++];

        char Peek() => _input[_position];
    }
}
