using System.Reflection;

namespace EntityReplication.DefaultValues
{
    public delegate TValue DefaultValueProviderDelegate<TId, TValue>(TId id, PropertyInfo propertyInfo);
}