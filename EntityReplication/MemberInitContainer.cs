using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityReplication
{
    internal class MemberInitContainer
    {
        internal ParameterExpression ParameterExpression { get; private set; }
        internal NewExpression ConstructorExpression { get; private set; }
        internal IEnumerable<MemberBinding> MemberBindings { get; private set; }

        public MemberInitContainer(ParameterExpression parameterExpression, NewExpression constructorExpression, IEnumerable<MemberBinding> memberBindings)
        {
            ParameterExpression = parameterExpression;
            ConstructorExpression = constructorExpression;
            MemberBindings = memberBindings;
        }
    }
}