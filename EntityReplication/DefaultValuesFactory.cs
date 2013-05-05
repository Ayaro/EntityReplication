using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication
{
    internal class DefaultValuesFactory<TId>
    {
        private readonly Dictionary<Type, Delegate> _defaultValueProviders = new Dictionary<Type, Delegate>();

        internal MemberBinding DefaultValueBinding(ParameterExpression parameterExpression, PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;
            Expression defaultValueExpression;

            Delegate @delegate;
            if (_defaultValueProviders.TryGetValue(propertyType, out @delegate))
            {
                var arguments = new Expression[] { parameterExpression, Expression.Constant(propertyInfo) };

                defaultValueExpression = @delegate.Target == null
                                       ? Expression.Call(@delegate.Method, arguments)
                                       : Expression.Call(Expression.Constant(@delegate.Target), @delegate.Method, arguments);
            }
            else
            {
                defaultValueExpression = Expression.Default(propertyType);
            }

            return Expression.Bind(propertyInfo, defaultValueExpression);
        }

        internal void SetDefaultValueProvider<TValue>(DefaultValueProviderDelegate<TId, TValue> defaultValueProviderDelegate)
        {
            _defaultValueProviders[typeof(TValue)] = defaultValueProviderDelegate;
        }
    }
}