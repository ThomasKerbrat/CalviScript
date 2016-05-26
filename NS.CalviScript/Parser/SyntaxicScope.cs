using System;
using System.Collections.Generic;

namespace NS.CalviScript
{
    class SyntaxicScope
    {
        Stack<Dictionary<string, VariableDeclarationExpression>> _scopes;

        internal SyntaxicScope()
        {
            _scopes = new Stack<Dictionary<string, VariableDeclarationExpression>>();
            _scopes.Push(new Dictionary<string, VariableDeclarationExpression>());
        }

        public IExpression Declare(string identifier)
        {
            VariableDeclarationExpression existing;
            var currentScope = _scopes.Peek();

            if (currentScope.TryGetValue(identifier, out existing))
                return new ErrorExpression("Duplicate identifier declaration: " + identifier);

            existing = new VariableDeclarationExpression(identifier);
            currentScope.Add(identifier, existing);
            return existing;
        }

        public LookUpExpression LookUp(string identifier)
        {
            VariableDeclarationExpression existing = null;

            foreach (var scope in _scopes)
                if (scope.TryGetValue(identifier, out existing))
                    break;

            return new LookUpExpression(identifier, existing);
        }

        public IDisposable OpenScope()
            => new ScopeCloser(this);

        class ScopeCloser : IDisposable
        {
            readonly SyntaxicScope _current;

            public ScopeCloser(SyntaxicScope scope)
            {
                _current = scope;
                _current._scopes.Push(new Dictionary<string, VariableDeclarationExpression>());
            }

            public void Dispose()
                => _current._scopes.Pop();
        }
    }
}
