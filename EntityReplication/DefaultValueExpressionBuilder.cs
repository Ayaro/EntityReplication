using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication
{
    internal class DefaultValueExpressionBuilder
    {
        private readonly ParameterExpression _parameterExpression;
        private readonly PropertyInfo _propertyInfo;

        public DefaultValueExpressionBuilder(ParameterExpression parameterExpression, PropertyInfo propertyInfo)
        {
            if (null == parameterExpression)
            {
                throw new ArgumentNullException("parameterExpression");
            }
            if (null == propertyInfo)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            _parameterExpression = parameterExpression;
            _propertyInfo = propertyInfo;
        }

        public Expression BuildExpression(Delegate defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                return Expression.Default(_propertyInfo.PropertyType);
            }

            return buildMethodCallExpression(defaultValueProvider.Method, defaultValueProvider.Target);
        }

        private Expression buildMethodCallExpression(MethodInfo method, object target)
        {
            if (target == null)
            {
                return buildStaticMethodCallExpression(method);
            }

            return buildInstanceMethodCallExpression(method, target);
        }

        private Expression buildStaticMethodCallExpression(MethodInfo method)
        {
            Expression[] arguments = getArgumentExpressions();

            return Expression.Call(method, arguments);
        }

        private MethodCallExpression buildInstanceMethodCallExpression(MethodInfo method, object target)
        {
            Expression[] arguments = getArgumentExpressions();

            return Expression.Call(Expression.Constant(target), method, arguments);
        }

        private Expression[] getArgumentExpressions()
        {
            return new Expression[]
                {
                    _parameterExpression, 
                    Expression.Constant(_propertyInfo)
                };
        }
    }
}