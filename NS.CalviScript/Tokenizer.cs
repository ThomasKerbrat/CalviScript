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



        public bool IsEnd => _position >= _input.Length;

        bool IsComment => _position < _input.Length - 1 && Peek() == '/' && Peek(1) == '/';

        public bool IsWhiteSpace => char.IsWhiteSpace(Peek());

        private bool IsNumber => char.IsDigit(Peek()) && Peek() != '0';



        public Token GetNextToken()
        {
            if (IsEnd) return new Token(TokenType.End);

            while (IsWhiteSpace || IsComment)
            {
                if (IsWhiteSpace) HandleWhiteSpace();
                if (IsComment) HandleComment();
            }

            Token result;
            if (Peek() == '+') result = HandleSimpleToken(TokenType.Plus);
            else if (Peek() == '-') result = HandleSimpleToken(TokenType.Minus);
            else if (Peek() == '*') result = HandleSimpleToken(TokenType.Mult);
            else if (Peek() == '/') result = HandleSimpleToken(TokenType.Div);
            else if (Peek() == '%') result = HandleSimpleToken(TokenType.Modulo);
            else if (Peek() == '(') result = HandleSimpleToken(TokenType.LeftParenthesis);
            else if (Peek() == ')') result = HandleSimpleToken(TokenType.RightParenthesis);
            else if (IsNumber) result = HandleNumber();
            else result = new Token(TokenType.Error, Peek());

            return result;
        }

        private void HandleWhiteSpace()
        {
            Debug.Assert(IsWhiteSpace);
            do
            {
                Forward();
            } while (!IsEnd && IsWhiteSpace);
        }

        void HandleComment()
        {
            Debug.Assert(IsComment);
            do
            {
                Forward();
            } while (!IsEnd && Peek() != '\n' && Peek() != '\r');
        }

        Token HandleSimpleToken(TokenType type)
        {
            Forward();
            return new Token(type, Peek(-1));
        }

        private Token HandleNumber()
        {
            Debug.Assert(IsNumber);

            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(Peek());
                Forward();
            } while (!IsEnd && char.IsDigit(Peek()));

            return new Token(TokenType.Number, sb.ToString());
        }



        char Read() => _input[_position++];

        char Peek(int offset) => _input[_position + offset];

        char Peek() => Peek(0);

        void Forward() => _position++;
    }
}
