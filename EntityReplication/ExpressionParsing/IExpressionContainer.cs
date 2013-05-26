using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal interface IExpressionContainer
    {
        ParameterExpression ParameterExpression { get; }
        NewExpression ConstructorExpression { get; }
        IEnumerable<MemberBinding> MemberBindings { get; }
    }
}