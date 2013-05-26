using System.Linq.Expressions;

namespace EntityReplication.ExpressionParsing
{
    internal static class ExpressionContainerFactory
    {
        internal static IExpressionContainer Create(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null)
            {
                return ExpressionContainer.Empty;
            }

            return containerForLambdaExpression(lambdaExpression);
        }

        private static IExpressionContainer containerForLambdaExpression(LambdaExpression lambdaExpression)
        {
            switch (lambdaExpression.Body.NodeType)
            {
                case ExpressionType.MemberInit:
                    return new MemberInitExpressionContainer(lambdaExpression);

                case ExpressionType.New:
                    return new NewExpressionContainer(lambdaExpression);

                default:
                    return ExpressionContainer.Empty;
            }
        }
    }
}