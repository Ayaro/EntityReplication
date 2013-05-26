using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal class ExpressionContainer : IExpressionContainer
    {
        private static ExpressionContainer _empty;
        internal static ExpressionContainer Empty { get { return _empty ?? (_empty = new ExpressionContainer()); } }

        public ParameterExpression ParameterExpression { get; private set; }
        public NewExpression ConstructorExpression { get; private set; }

        private readonly IEnumerable<MemberBinding> _memberBindings;
        public IEnumerable<MemberBinding> MemberBindings { get { return _memberBindings ?? Enumerable.Empty<MemberBinding>(); } }

        private ExpressionContainer() { }

        public ExpressionContainer(ParameterExpression parameterExpression, NewExpression constructorExpression, IEnumerable<MemberBinding> memberBindings)
        {
            ParameterExpression = parameterExpression;
            ConstructorExpression = constructorExpression;
            _memberBindings = memberBindings;
        }
    }
}