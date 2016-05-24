using System.Collections.Generic;

namespace NS.CalviScript
{
    /// <summary>
    /// program: (statement ';')* EOI
    /// statement: variableDeclaration | expression
    /// *variableDeclaration: VAR IDENTIFIER '=' expression
    /// expression: mathExpression ('?' expression ':' expression)?
    /// mathExpression: term (('+' | '-') term)*
    /// term: factor (('*' | '/' | '%') factor)*
    /// factor: '-'? positiveFactor
    /// positiveFactor: NUMBER | IDENTIFIER | ('(' expression ')')
    /// </summary>
    public class Parser
    {
        readonly Tokenizer _tokenizer;

        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _tokenizer.GetNextToken();
        }

        #region Entry Points
        public IExpression ParseProgram()
        {
            var statements = new List<IExpression>();

            while (!_tokenizer.MatchToken(TokenType.End))
            {
                statements.Add(Statement());
                if (!_tokenizer.MatchToken(TokenType.SemiColon)) return CreateErrorExpression(";");
            }

            return new ProgramExpression(statements);
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

        public static IExpression Parse(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }
        #endregion

        #region Grammar Methods
        IExpression Statement()
        {
            if (_tokenizer.CurrentToken.Type == TokenType.Var)
                return VariableDeclaration();
            return ParseExpression();
        }

        IExpression VariableDeclaration()
        {
            if (!_tokenizer.MatchToken(TokenType.Var)) CreateErrorExpression("var");

            Token token;
            if (!_tokenizer.MatchToken(TokenType.Var, out token)) CreateErrorExpression(";");
            if (!_tokenizer.MatchToken(TokenType.Equal)) CreateErrorExpression("=");

            return new VariableDeclarationExpression(token.Value, ParseExpression());
        }

        IExpression Expression()
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
                    expression = CreateErrorExpression(":");
                }
            }

            return expression;
        }

        IExpression MathExpression()
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

        IExpression Term()
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

        IExpression Factor()
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

        IExpression PositiveFactor()
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
                    result = CreateErrorExpression(")");
                else
                    _tokenizer.GetNextToken();
            }
            else if (_tokenizer.MatchToken(TokenType.Identifier, out token))
            {
                result = new LookUpExpression(token.Value);
            }
            else
            {
                return new ErrorExpression(string.Format("Unexpected token: {0}", _tokenizer.CurrentToken.Value));
            }

            return result;
        }
        #endregion

        ErrorExpression CreateErrorExpression(string expected)
        {
            return new ErrorExpression(string.Format(
                "Expected <{0}>, but <{1}> found.", expected, _tokenizer.CurrentToken.Value));
        }
    }
}
