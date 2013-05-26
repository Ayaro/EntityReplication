using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal class NewExpressionContainer : ParameterizedExpressionContainerBase<NewExpression>
    {
        public override NewExpression ConstructorExpression { get { return Body; } }
        public override IEnumerable<MemberBinding> MemberBindings { get { return Enumerable.Empty<MemberBinding>(); } }

        public NewExpressionContainer(LambdaExpression lambdaExpression)
            : base(lambdaExpression)
        {
        }
    }
}