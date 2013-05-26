using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal abstract class ParameterizedExpressionContainerBase<TBody> : IExpressionContainer
        where TBody : Expression
    {
        protected TBody Body { get; private set; }
        public ParameterExpression ParameterExpression { get; private set; }
        public abstract NewExpression ConstructorExpression { get; }
        public abstract IEnumerable<MemberBinding> MemberBindings { get; }

        protected ParameterizedExpressionContainerBase(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null)
            {
                throw new ArgumentNullException("lambdaExpression");
            }

            Body = (TBody)lambdaExpression.Body;
            ParameterExpression = lambdaExpression.Parameters.SingleOrDefault();
        }
    }
}