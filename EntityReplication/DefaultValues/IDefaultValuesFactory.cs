using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication.DefaultValues
{
    internal interface IDefaultValuesFactory<TIdentifier>
    {
        void SetDefaultValueProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProviderDelegate);
        MemberBinding DefaultValueBinding(ParameterExpression parameterExpression, PropertyInfo propertyInfo);
    }
}