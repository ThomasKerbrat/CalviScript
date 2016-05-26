using System;
using System.Collections.Generic;

namespace NS.CalviScript
{
    class SyntaxicScope
    {
        Dictionary<string, VariableDeclarationExpression> _scope;

        public SyntaxicScope()
        {
            _scope = new Dictionary<string, VariableDeclarationExpression>();
        }

        public IExpression Declare(string identifier)
        {
            VariableDeclarationExpression existing;
            if (_scope.TryGetValue(identifier, out existing))
                return new ErrorExpression("Duplicate identifier declaration: " + identifier);

            existing = new VariableDeclarationExpression(identifier);
            _scope.Add(identifier, existing);
            return existing;
        }

        public LookUpExpression LookUp(string identifier)
        {
            VariableDeclarationExpression existing;
            _scope.TryGetValue(identifier, out existing);
            return new LookUpExpression(identifier, existing);
        }
    }
}
