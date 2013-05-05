using System.Reflection;

namespace EntityReplication
{
    public delegate TValue DefaultValueProviderDelegate<TId, TValue>(TId id, PropertyInfo propertyInfo);
}