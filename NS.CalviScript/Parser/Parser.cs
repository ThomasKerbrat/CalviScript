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
        IExpression ParseProgram()
        {
            List<IExpression> statements = new List<IExpression>();

            while (!_tokenizer.MatchToken(TokenType.End))
            {
                var statement = StatementList(expectCurly: false, allowReturn: false);
                if (statement is ErrorExpression)
                    return statement;
                statements.Add(statement);
            }

            return statements.Count == 1 && statements[0] is BlockExpression
                ? statements[0]
                : new BlockExpression(statements);
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

        IExpression ParseExpression()
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
        IExpression StatementList(bool expectCurly, bool allowReturn = false)
        {
            var statements = new List<IExpression>();
            var matchedCurly = false;

            while (!(matchedCurly = _tokenizer.MatchToken(TokenType.CloseCurly)) && !_tokenizer.MatchToken(TokenType.End))
            {
                var statement = ScopeBlock(expected: _tokenizer.CurrentToken.Type == TokenType.OpenCurly)
                    ?? Statement(allowReturn: allowReturn);
                if (statement is ErrorExpression)
                    return statement;
                statements.Add(statement);
            }

            if (!expectCurly && matchedCurly)
                return CreateUnexpectedTokenExpression("}");

            return new BlockExpression(statements);
        }

        IExpression ScopeBlock(bool expected)
        {
            if (!_tokenizer.MatchToken(TokenType.OpenCurly))
                return expected ? CreateErrorExpression("{") : null;

            using (_syntaxicScope.OpenScope())
            {
                return StatementList(expectCurly: expected, allowReturn: false);
            }
        }

        IExpression Statement(bool allowReturn = false)
        {
            IExpression expression = VariableDeclaration()
                ?? While(false)
                ?? Return()
                ?? Expression();

            if (expression == null)
                return CreateErrorExpression("statement");

            if (!allowReturn && expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;
                if (unaryExpression.OperatorType == TokenType.Return)
                    return CreateUnexpectedTokenExpression("return");
            }

            _tokenizer.MatchToken(TokenType.SemiColon);

            // Strict mode:
            //if (!_tokenizer.MatchToken(TokenType.SemiColon))
            //    return CreateErrorExpression(";");

            return expression;
        }

        IExpression Return()
        {
            if (!_tokenizer.MatchToken(TokenType.Return))
                return null;

            var statement = Expression();
            if (statement is ErrorExpression)
                return statement;

            return new UnaryExpression(TokenType.Return, statement);
        }

        IExpression VariableDeclaration()
        {
            // If no "var", return error.
            if (!_tokenizer.MatchToken(TokenType.Var))
                return null;

            Token token;

            // If no "identifier", return error.
            if (!_tokenizer.MatchToken(TokenType.Identifier, out token))
                return CreateErrorExpression("identifier");

            // Ask the syntaxic scope to handle the variable declaration.
            IExpression variableDeclaration = _syntaxicScope.Declare(token.Value);
            if (variableDeclaration is ErrorExpression)
                return variableDeclaration;

            return Assign((VariableDeclarationExpression)variableDeclaration);
        }

        IExpression Assign(IIdentifierExpression identifier)
        {
            // If no "=", return error.
            if (!_tokenizer.MatchToken(TokenType.Equal))
                return identifier;

            // Parse the expression.
            IExpression expression = Expression();
            if (expression == null)
                return CreateErrorExpression("expression");

            return new AssignExpression(identifier, expression);
        }

        IExpression Expression()
        {
            if (_tokenizer.MatchToken(TokenType.Function))
                return FunctionDeclaration();

            var expression = MathExpression();

            if (_tokenizer.MatchToken(TokenType.QuestionMark))
            {
                IExpression trueExpression = Expression();
                if (_tokenizer.MatchToken(TokenType.Colon))
                {
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

            Token operatorToken = _tokenizer.CurrentToken;
            while (_tokenizer.MatchTermOperator(out operatorToken))
            {
                IExpression right = Term();
                left = new BinaryExpression(operatorToken.Type, left, right);
            }

            return left;
        }

        IExpression Term()
        {
            IExpression left = Factor();

            Token operatorToken = _tokenizer.CurrentToken;
            while (_tokenizer.MatchFactorOperator(out operatorToken))
            {
                IExpression right = Factor();
                left = new BinaryExpression(operatorToken.Type, left, right);
            }

            return left;
        }

        IExpression Factor()
        {
            bool isMinusExpression = _tokenizer.MatchToken(TokenType.Minus);
            IExpression expression;

            if (isMinusExpression)
            {
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
                result = new ConstantExpression(int.Parse(token.Value));
            }
            else if (_tokenizer.MatchToken(TokenType.LeftParenthesis))
            {
                result = Expression();
                if (!_tokenizer.MatchToken(TokenType.RightParenthesis))
                    result = CreateErrorExpression(")");
            }
            else if (_tokenizer.MatchToken(TokenType.Identifier, out token))
            {
                result = FunctionCall(identifier: token.Value, expected: false)
                    ?? Assign(_syntaxicScope.LookUp(token.Value));
            }
            else
            {
                result = CreateUnexpectedErrorExpression();
            }

            return result;
        }

        IExpression FunctionDeclaration()
        {
            // Opening parenthesis
            if (!_tokenizer.MatchToken(TokenType.LeftParenthesis))
                return CreateErrorExpression("(");

            // Params
            var parameters = new List<VariableDeclarationExpression>();
            Token identifier;
            while (_tokenizer.MatchToken(TokenType.Identifier, out identifier))
            {
                IExpression declarationOrError = _syntaxicScope.Declare(identifier.Value);

                if (declarationOrError is ErrorExpression)
                    return declarationOrError;
                parameters.Add((VariableDeclarationExpression)declarationOrError);

                _tokenizer.MatchToken(TokenType.Coma);
            }

            // Closing parenthesis
            if (!_tokenizer.MatchToken(TokenType.RightParenthesis))
                return CreateErrorExpression(")");

            // Body
            if (!_tokenizer.MatchToken(TokenType.OpenCurly))
                return CreateErrorExpression("{");
            IExpression body = null;
            using (_syntaxicScope.OpenScope())
            {
                body = StatementList(expectCurly: true, allowReturn: true);
            }
            if (body == null || body is ErrorExpression)
                return body;

            return new FunctionDeclarationExpression(parameters, (BlockExpression)body);
        }

        IExpression FunctionCall(string identifier, bool expected)
        {
            // Opening parenthesis
            if (!_tokenizer.MatchToken(TokenType.LeftParenthesis))
                return expected
                    ? CreateErrorExpression("(")
                    : null;

            // Name
            var name = _syntaxicScope.LookUp(identifier);

            // Arguments & Closing parenthesis
            var arguments = new List<IExpression>();
            while (!_tokenizer.MatchToken(TokenType.RightParenthesis))
            {
                IExpression expression = Expression();
                if (expression == null || expression is ErrorExpression)
                    return expression;
                arguments.Add(expression);
                _tokenizer.MatchToken(TokenType.Coma);
            }

            return new FunctionCallExpression(name, arguments);
        }

        IExpression While(bool expected)
        {
            if (!_tokenizer.MatchToken(TokenType.While))
                return expected ? CreateErrorExpression("while") : null;

            // Match the condition expression with parenthesis.
            if (!_tokenizer.MatchToken(TokenType.LeftParenthesis))
                return CreateErrorExpression("(");
            var condition = Expression();
            if (!_tokenizer.MatchToken(TokenType.RightParenthesis))
                return CreateErrorExpression(")");

            // Match the body with optional curly braces.
            if (!_tokenizer.MatchToken(TokenType.OpenCurly))
                return CreateErrorExpression("{");
            IExpression body;
            using (_syntaxicScope.OpenScope())
            {
                body = StatementList(expectCurly: true, allowReturn: false);
            }
            if (body == null || body is ErrorExpression)
                return body;

            return new WhileExpression(condition, (BlockExpression)body);
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

        ErrorExpression CreateUnexpectedTokenExpression(string expected)
        {
            return new ErrorExpression(string.Format(
                "Unexpected token <{0}>.", expected));
        }
    }
}
