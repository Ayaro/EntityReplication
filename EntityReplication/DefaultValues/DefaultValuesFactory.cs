using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication.DefaultValues
{
    internal class DefaultValuesFactory<TIdentifier> : IDefaultValuesFactory<TIdentifier>
    {
        private readonly Dictionary<Type, Delegate> _defaultValueProviders = new Dictionary<Type, Delegate>();

        public void SetDefaultValueProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProviderDelegate)
        {
            _defaultValueProviders[typeof(TValue)] = defaultValueProviderDelegate;
        }

        public MemberBinding DefaultValueBinding(ParameterExpression parameterExpression, PropertyInfo propertyInfo)
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
    }
}