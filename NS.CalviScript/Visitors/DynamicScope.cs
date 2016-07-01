using System;
using System.Collections.Generic;

namespace NS.CalviScript
{
    // TODO: Can make SyntaxicScope and DynamicScope a generic Scope<T>.
    class DynamicScope
    {
        Stack<Dictionary<VariableDeclarationExpression, BaseValue>> _values;

        internal DynamicScope()
        {
            _values = new Stack<Dictionary<VariableDeclarationExpression, BaseValue>>();
            _values.Push(new Dictionary<VariableDeclarationExpression, BaseValue>());
        }

        public void Register(VariableDeclarationExpression expression, BaseValue value)
        {
            _values.Peek().Add(expression, value);
        }

        public BaseValue FindRegistered(VariableDeclarationExpression identifier)
        {
            BaseValue existing = null;

            foreach (var scope in _values)
            {
                if (scope.TryGetValue(identifier, out existing))
                {
                    break;
                }
            }

            if (existing == null)
                throw new Exception("Variable are necessarily registered.");

            return existing;
        }

        public BaseValue SetValue(VariableDeclarationExpression identifier, BaseValue value)
        {
            BaseValue existing = null;

            foreach (var scope in _values)
            {
                if (scope.ContainsKey(identifier))
                {
                    scope[identifier] = value;
                    existing = value;
                    break;
                }
            }

            if (existing == null)
                throw new Exception("Variable are necessarily registered.");

            return _values.Peek()[identifier] = value;
        }

        public IDisposable OpenScope()
            => new ScopeCloser(this);

        class ScopeCloser : IDisposable
        {
            readonly DynamicScope _current;

            public ScopeCloser(DynamicScope scope)
            {
                _current = scope;
                _current._values.Push(new Dictionary<VariableDeclarationExpression, BaseValue>());
            }

            public void Dispose()
                => _current._values.Pop();
        }
    }
}
