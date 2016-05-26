using System.Collections.Generic;

namespace NS.CalviScript
{
    public class Parser
    {
        readonly Tokenizer _tokenizer;
        readonly SyntaxicScope _syntaxicScope;

        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _syntaxicScope = new SyntaxicScope();
            _tokenizer.GetNextToken();
        }

        #region Entry Points
        /// <summary>
        /// Parse a program as defined in the "program" rule in formal grammar.
        /// </summary>
        /// <returns>The program AST.</returns>
        public IExpression ParseProgram()
        {
            IExpression result = Program();
            Token token;
            if (!_tokenizer.MatchToken(TokenType.End, out token))
                return CreateErrorExpression("EOI");
            return result;
        }

        /// <summary>
        /// Parse a program as defined in the "program" rule in formal grammar. 
        /// This methods instanciate a <see cref="Tokenizer"/> and a <see cref="Parser"/>.
        /// </summary>
        /// <param name="input">A string input to give to the <see cref="Tokenizer"/>.</param>
        /// <returns>The program expression.</returns>
        public static IExpression ParseProgram(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            return parser.ParseProgram();
        }

        /// <summary>
        /// Parse a program as defined in the "expression" rule in formal grammar.
        /// </summary>
        /// <returns>The expression AST.</returns>
        public IExpression ParseExpression()
        {
            IExpression result = Expression();
            Token token;
            if (!_tokenizer.MatchToken(TokenType.End, out token))
                return CreateErrorExpression("EOI");
            return result;
        }

        /// <summary>
        /// Parse a program as defined in the "expression" rule in formal grammar.
        /// This methods instanciate a <see cref="Tokenizer"/> and a <see cref="Parser"/>.
        /// </summary>
        /// <param name="input">A string input to give to the <see cref="Tokenizer"/>.</param>
        /// <returns>The expression AST.</returns>
        public static IExpression ParseExpression(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }
        #endregion

        #region Grammar Methods
        IExpression Program()
        {
            var statements = new List<IExpression>();

            while (!_tokenizer.MatchToken(TokenType.End))
            {
                statements.Add(Statement());
                if (!_tokenizer.MatchToken(TokenType.SemiColon))
                    return CreateErrorExpression(";");
                _tokenizer.GetNextToken();
            }

            return new BlockExpression(statements);
        }

        IExpression Statement()
        {
            if (_tokenizer.CurrentToken.Type == TokenType.Var)
                return VariableDeclaration();
            return Expression();
        }

        IExpression VariableDeclaration()
        {
            // If no "var", return error.
            if (!_tokenizer.MatchToken(TokenType.Var))
                return CreateErrorExpression("var");

            Token token;

            // If no "identifier", return error.
            _tokenizer.GetNextToken();
            if (!_tokenizer.MatchToken(TokenType.Identifier, out token))
                return CreateErrorExpression("identifier");

            // Ask the syntaxic scope to handle the variable declaration.
            IExpression variableDeclaration = _syntaxicScope.Declare(token.Value);
            if (variableDeclaration is ErrorExpression)
                return variableDeclaration;
            var _variableDeclaration = (VariableDeclarationExpression)variableDeclaration;

            // If no "=", return error.
            _tokenizer.GetNextToken();
            if (!_tokenizer.MatchToken(TokenType.Equal))
                return _variableDeclaration;

            // Parse the expression.
            _tokenizer.GetNextToken();
            IExpression result = ParseExpression();
            if (result == null)
                return CreateErrorExpression("expression");

            _tokenizer.GetNextToken();
            return new VariableDeclarationExpression(token.Value);
        }

        IExpression Expression()
        {
            var expression = MathExpression();

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
                _tokenizer.GetNextToken();
            }
            else if (_tokenizer.MatchToken(TokenType.Identifier, out token))
            {
                result = _syntaxicScope.LookUp(token.Value);
                //result = new LookUpExpression(token.Value, _syntaxicScope.Find(token.Value));
                _tokenizer.GetNextToken();
            }
            else
                return CreateUnexpectedErrorExpression();

            return result;
        }
        #endregion

        ErrorExpression CreateErrorExpression(string expected)
        {
            return new ErrorExpression(string.Format(
                "Expected <{0}>, but <{1}> found.", expected, _tokenizer.CurrentToken.Value));
        }

        ErrorExpression CreateUnexpectedErrorExpression()
        {
            return new ErrorExpression(string.Format(
                "Unexpected token <{0}>.", _tokenizer.CurrentToken.Value));
        }
    }
}
