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
            List<IExpression> statements = new List<IExpression>();

            while (!_tokenizer.MatchToken(TokenType.End))
            {
                var statement = Block(false) ?? Statement();
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
        IExpression Block(bool expected)
        {
            if (!_tokenizer.MatchToken(TokenType.OpenCurly))
                return expected ? CreateErrorExpression("{") : null;

            var statements = new List<IExpression>();
            using (_syntaxicScope.OpenScope())
            {
                while (!_tokenizer.MatchToken(TokenType.CloseCurly))
                {
                    var statement = Block(false) ?? Statement();
                    if (statement is ErrorExpression) return statement;
                    statements.Add(statement);
                }
            }

            if (!_tokenizer.MatchToken(TokenType.OpenCurly))
                return CreateErrorExpression("}");

            return new BlockExpression(statements);
        }

        IExpression Statement()
        {
            IExpression expression = VariableDeclaration()
                ?? Expression();

            if (expression == null)
                return CreateErrorExpression("statement");

            if (!_tokenizer.MatchToken(TokenType.SemiColon))
                return CreateErrorExpression(";");
            
            return expression;
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
                result = Assign(_syntaxicScope.LookUp(token.Value));
            }
            else
            {
                result = CreateUnexpectedErrorExpression();
            }

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
