using System;

namespace NS.CalviScript
{
    public class Parser
    {
        readonly Tokenizer _tokenizer;

        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _tokenizer.GetNextToken();
        }

        public IExpression ParseOperation()
        {
            IExpression expression = Expression();
            Token token;
            if (!_tokenizer.MatchToken(TokenType.End, out token))
            {
                return new ErrorExpression(string.Format("Expected end of input, but {0} found.", _tokenizer.CurrentToken.Type.ToString()));
            }
            return expression;
        }

        public IExpression Expression()
        {
            IExpression left = Term();

            Token token = _tokenizer.CurrentToken;
            while (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                _tokenizer.GetNextToken();
                IExpression right = Term();
                left = new BinaryExpression(token.Type, left, right);
                token = _tokenizer.CurrentToken;
            }

            return left;
        }

        public IExpression Term()
        {
            IExpression left = Factor();

            Token token = _tokenizer.CurrentToken;
            while (token.Type == TokenType.Mult || token.Type == TokenType.Div || token.Type == TokenType.Modulo)
            {
                _tokenizer.GetNextToken();
                IExpression right = Factor();
                left = new BinaryExpression(token.Type, left, right);
                token = _tokenizer.CurrentToken;
            }

            return left;
        }

        public IExpression Factor()
        {
            IExpression result;
            Token token;

            if (_tokenizer.MatchToken(TokenType.LeftParenthesis, out token))
            {
                result = Expression();
                if (!_tokenizer.MatchToken(TokenType.RightParenthesis, out token))
                {
                    var error = "Expected closing parenthesis, but {0} found.";
                    result = new ErrorExpression(string.Format(error, _tokenizer.CurrentToken.Type.ToString()));
                }
            }
            else
            {
                result = new NumberExpression(int.Parse(_tokenizer.CurrentToken.Value));
                _tokenizer.GetNextToken();
            }

            return result;
        }
    }
}
