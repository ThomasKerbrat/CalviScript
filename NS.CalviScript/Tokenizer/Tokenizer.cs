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

        #region Properties
        public Token CurrentToken { get; private set; }

        bool IsEnd => _position >= _input.Length;

        bool IsComment => _position < _input.Length - 1 && Peek() == '/' && Peek(1) == '/';

        bool IsWhiteSpace => char.IsWhiteSpace(Peek());

        bool IsNumber => char.IsDigit(Peek());

        bool IsIdentifier => char.IsLetter(Peek()) || Peek() == '_';
        #endregion

        public Token GetNextToken()
        {
            if (IsEnd) return CurrentToken = new Token(TokenType.End);

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
            else if (Peek() == '?') result = HandleSimpleToken(TokenType.QuestionMark);
            else if (Peek() == ':') result = HandleSimpleToken(TokenType.Colon);
            else if (Peek() == '=') result = HandleSimpleToken(TokenType.Equal);
            else if (Peek() == ';') result = HandleSimpleToken(TokenType.SemiColon);
            else if (IsNumber) result = HandleNumber();
            else if (IsIdentifier) result = HandleIdentifier();
            else result = new Token(TokenType.Error, Peek());

            CurrentToken = result;
            return result;
        }

        #region Handle Methods
        void HandleWhiteSpace()
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

        Token HandleNumber()
        {
            Debug.Assert(IsNumber);

            if (Peek() == '0')
            {
                Forward();
                if (!IsEnd && IsNumber) return new Token(TokenType.Error, Peek());
                return new Token(TokenType.Number, '0');
            }

            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(Peek());
                Forward();
            } while (!IsEnd && char.IsDigit(Peek()));

            return new Token(TokenType.Number, sb.ToString());
        }

        Token HandleIdentifier()
        {
            Debug.Assert(IsIdentifier);

            var sb = new StringBuilder();
            do
            {
                sb.Append(Peek());
                Forward();
            } while (!IsEnd && (IsIdentifier || char.IsDigit(Peek())));

            string identifier = sb.ToString();
            if (identifier == "var") return new Token(TokenType.Var, identifier);
            else return new Token(TokenType.Identifier, identifier);
        }
        #endregion

        #region Move methods
        char Read() => _input[_position++];

        char Peek(int offset) => _input[_position + offset];

        char Peek() => Peek(0);

        void Forward() => _position++;
        #endregion

        #region Parser Helpers
        public bool MatchNumber(out Token token)
        {
            return MatchToken(TokenType.Number, out token);
        }

        public bool MatchTermOperator(out Token token)
            => MatchOperator(type => type == TokenType.Plus || type == TokenType.Minus, out token);

        public bool MatchFactorOperator(out Token token)
            => MatchOperator(type => type == TokenType.Mult || type == TokenType.Div || type == TokenType.Modulo, out token);

        public bool MatchOperator(Func<TokenType, bool> predicate, out Token token)
        {
            if (predicate(CurrentToken.Type))
            {
                token = CurrentToken;
                return true;
            }

            token = null;
            return false;
        }

        public bool MatchToken(TokenType type)
        {
            Token token;
            return MatchToken(type, out token);
        }

        public bool MatchToken(TokenType type, out Token token)
        {
            if (type == CurrentToken.Type)
            {
                token = CurrentToken;
                return true;
            }

            token = null;
            return false;
        }
        #endregion
    }
}
