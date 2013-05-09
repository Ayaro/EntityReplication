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
            Expression defaultValueExpression = buildBindingExpression(parameterExpression, propertyInfo);
            MemberBinding memberBinding = Expression.Bind(propertyInfo, defaultValueExpression);

            return memberBinding;
        }

        private Expression buildBindingExpression(ParameterExpression parameterExpression, PropertyInfo propertyInfo)
        {
            var defaultValueExpressionBuilder = new DefaultValueExpressionBuilder(parameterExpression, propertyInfo);
            Delegate defaultValueProvider = defaultValueProviderOrNull(propertyInfo.PropertyType);

            return defaultValueExpressionBuilder.BuildExpression(defaultValueProvider);
        }

        private Delegate defaultValueProviderOrNull(Type type)
        {
            Delegate defaultValueProvider;
            _defaultValueProviders.TryGetValue(type, out defaultValueProvider);

            return defaultValueProvider;
        }

        internal void SetDefaultValueProvider<TValue>(DefaultValueProviderDelegate<TId, TValue> defaultValueProviderDelegate)
        {
            _defaultValueProviders[typeof(TValue)] = defaultValueProviderDelegate;
        }
    }
}