using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal class MemberInitExpressionContainer : ParameterizedExpressionContainerBase<MemberInitExpression>
    {
        public override NewExpression ConstructorExpression { get { return Body.NewExpression; } }
        public override IEnumerable<MemberBinding> MemberBindings { get { return Body.Bindings; } }

        public MemberInitExpressionContainer(LambdaExpression lambdaExpression)
            : base(lambdaExpression)
        {
        }
    }
}