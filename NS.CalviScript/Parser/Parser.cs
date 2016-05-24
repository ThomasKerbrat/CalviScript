namespace NS.CalviScript
{
    /// <summary>
    /// start: expression EOI
    /// expression: mathExpression ('?' expression ':' expression)?
    /// mathExpression: term (('+' | '-') term)*
    /// term: factor (('*' | '/' | '%') factor)*
    /// factor: '-'? positiveFactor
    /// positiveFactor: NUMBER | ('(' expression ')')
    /// </summary>
    public class Parser
    {
        readonly Tokenizer _tokenizer;

        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _tokenizer.GetNextToken();
        }

        public static IExpression Parse(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }

        public IExpression ParseExpression()
        {
            IExpression expression = Expression();
            Token token;
            if (!_tokenizer.MatchToken(TokenType.End, out token))
            {
                return new ErrorExpression(string.Format("Expected end of input, but {0} found.", _tokenizer.CurrentToken.Type.ToString()));
            }
            return expression;
        }

        #region Grammar Methods
        public IExpression Expression()
        {
            var expression = MathExpression();
            Token token;

            if (_tokenizer.MatchToken(TokenType.QuestionMark))
            {
                _tokenizer.GetNextToken();
                IExpression trueExpression = Expression();
                if (_tokenizer.MatchToken(TokenType.Colon))
                {
                    _tokenizer.GetNextToken();
                    IExpression falseExpresion = Expression();
                    expression = new TernaryExpression(expression, trueExpression, falseExpresion);
                }
                else
                {
                    expression = new ErrorExpression(string.Format("Expected colon but found: {0}", _tokenizer.CurrentToken));
                }
            }

            return expression;
        }

        public IExpression MathExpression()
        {
            IExpression left = Term();

            Token token = _tokenizer.CurrentToken;
            while (_tokenizer.MatchTermOperator(out token))
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
            while (_tokenizer.MatchFactorOperator(out token))
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
            bool isMinusExpression = _tokenizer.MatchToken(TokenType.Minus);
            IExpression expression;

            if (isMinusExpression)
            {
                _tokenizer.GetNextToken();
                expression = new UnaryExpression(TokenType.Minus, PositiveFactor());
            }
            else
            {
                expression = PositiveFactor();
            }

            return expression;
        }

        public IExpression PositiveFactor()
        {
            IExpression result;
            Token token;

            if (_tokenizer.MatchNumber(out token))
            {
                result = new ConstantExpression(int.Parse(_tokenizer.CurrentToken.Value));
                _tokenizer.GetNextToken();
            }
            else if (_tokenizer.MatchToken(TokenType.LeftParenthesis))
            {
                _tokenizer.GetNextToken();
                result = Expression();
                if (!_tokenizer.MatchToken(TokenType.RightParenthesis))
                {
                    var error = "Expected closing parenthesis, but {0} found.";
                    result = new ErrorExpression(string.Format(error, _tokenizer.CurrentToken.Type));
                }
                else
                {
                    _tokenizer.GetNextToken();
                }
            }
            else
            {
                return new ErrorExpression(string.Format("Unexpected token: {0}", token.Type));
            }

            return result;
        }
        #endregion
    }
}
