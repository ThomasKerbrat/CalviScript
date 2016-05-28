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
            else if (Peek() == '{') result = HandleSimpleToken(TokenType.OpenCurly);
            else if (Peek() == '}') result = HandleSimpleToken(TokenType.CloseCurly);
            else if (IsNumber) result = HandleNumber();
            else if (IsIdentifier) result = HandleIdentifier();
            else result = new Token(TokenType.Error, Peek());

            CurrentToken = result;
            return result;
        }

        #region Handle Methods
        /// <summary>
        /// Moves forward until a non-whitespace char is found.
        /// </summary>
        void HandleWhiteSpace()
        {
            Debug.Assert(IsWhiteSpace);
            do
            {
                Forward();
            } while (!IsEnd && IsWhiteSpace);
        }

        /// <summary>
        /// Moves forward until a new line or the end is found.
        /// </summary>
        void HandleComment()
        {
            Debug.Assert(IsComment);
            do
            {
                Forward();
            } while (!IsEnd && Peek() != '\n' && Peek() != '\r');
        }

        /// <summary>
        /// Creates a new <see cref="Token"/> and moves forward.
        /// </summary>
        /// <param name="type">Value in <see cref="TokenType"/> enum for the matched char.</param>
        /// <returns>A new <see cref="Token"/>.</returns>
        Token HandleSimpleToken(TokenType type)
        {
            Forward();
            return new Token(type, Peek(-1));
        }

        /// <summary>
        /// Creates a new <see cref="Token"/> and moves forward.
        /// The number should be a zero only, or start with a digit from 1 to 9 followed by any number of digit from 0 to 9.
        /// A number must not be immediately followed by an identifer.
        /// </summary>
        /// <returns>A new <see cref="Token"/>.</returns>
        Token HandleNumber()
        {
            Debug.Assert(IsNumber);

            if (Peek() == '0')
            {
                Forward();
                if (!IsEnd && (IsNumber || IsIdentifier)) return new Token(TokenType.Error, "0" + Peek());
                return new Token(TokenType.Number, '0');
            }

            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(Peek());
                Forward();
            } while (!IsEnd && IsNumber);

            if (!IsEnd && IsIdentifier)
                return new Token(TokenType.Error, sb.ToString() + Peek());

            return new Token(TokenType.Number, sb.ToString());
        }

        /// <summary>
        /// Creates a new <see cref="Token"/> and moves forward.
        /// It will match any identifier as defined by <see cref="IsIdentifier"/>.
        /// It will create special Token for "var": <see cref="TokenType.Var"/> and "while": <see cref="TokenType.While"/>.
        /// </summary>
        /// <returns>A new <see cref="Token"/>.</returns>
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
            if (identifier == "while") return new Token(TokenType.While, identifier);
            else return new Token(TokenType.Identifier, identifier);
        }
        #endregion

        #region Move methods
        /// <summary>
        /// Gets the current char and move the position forward.
        /// </summary>
        /// <returns>The char at current position.</returns>
        char Read()
            => _input[_position++];

        /// <summary>
        /// Gets the current char, but does **not** move the position forward.
        /// </summary>
        /// <returns>The char at current position.</returns>
        char Peek()
            => Peek(0);

        /// <summary>
        /// Gets the char at position + offset.
        /// Does **not** move the position forward.
        /// </summary>
        /// <param name="offset">How many char to read forward. 0 for current.</param>
        /// <returns>The char at offset + current position.</returns>
        char Peek(int offset)
            => _input[_position + offset];

        /// <summary>
        /// Increment the position by 1.
        /// </summary>
        void Forward()
            => _position++;
        #endregion

        #region Token Matchers
        /// <summary>
        /// Match the <see cref="CurrentToken"/> type to <see cref="TokenType.Number"/>.
        /// </summary>
        /// <param name="token">Variable that will be assigned during the match.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched with a <see cref="TokenType.Number"/>.</returns>
        public bool MatchNumber(out Token token)
            => MatchToken(TokenType.Number, out token);

        /// <summary>
        /// Match the <see cref="CurrentToken"/> type to one of the two possible term operators: "+" and "-".
        /// </summary>
        /// <param name="token">Variable that will be assigned during the match.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched with one of the two possible term operators.</returns>
        public bool MatchTermOperator(out Token token)
            => MatchToken(type => type == TokenType.Plus || type == TokenType.Minus, out token);

        /// <summary>
        /// Match the <see cref="CurrentToken"/> type to one of the three possible factor operators: "*", "/" and "%".
        /// </summary>
        /// <param name="token">Variable that will be assigned during the match.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched with one of the three possible factor operators.</returns>
        public bool MatchFactorOperator(out Token token)
            => MatchToken(type => type == TokenType.Mult || type == TokenType.Div || type == TokenType.Modulo, out token);

        /// <summary>
        /// Calls <see cref="MatchToken(TokenType, out Token)"/> but discard the Token value assigned in the method.
        /// </summary>
        /// <param name="type">Type to match against.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched with the provided Token type.</returns>
        public bool MatchToken(TokenType type, Token token = null)
            => MatchToken(type, out token);

        /// <summary>
        /// Match the <see cref="CurrentToken"/> type to any <see cref="TokenType"/> provided.
        /// </summary>
        /// <param name="type">Type to match against.</param>
        /// <param name="token">Variable that will be assigned during the match.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched with the provided Token type.</returns>
        public bool MatchToken(TokenType type, out Token token)
            => MatchToken(_type => _type == type, out token);

        /// <summary>
        /// Match the <see cref="CurrentToken"/> type to any <see cref="TokenType"/> with the provided predicate.
        /// </summary>
        /// <param name="predicate">A predicate that will determine if the <see cref="TokenType"/> has matched.</param>
        /// <param name="token">Variable that will be assigned during the match.</param>
        /// <returns>Returns if the <see cref="CurrentToken"/> type has matched according to the provided predicate.</returns>
        public bool MatchToken(Func<TokenType, bool> predicate, out Token token)
        {
            if (predicate(CurrentToken.Type))
            {
                token = CurrentToken;
                GetNextToken();
                return true;
            }

            token = null;
            return false;
        }
        #endregion
    }
}
