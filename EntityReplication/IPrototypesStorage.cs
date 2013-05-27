using System;
using System.Linq.Expressions;

namespace EntityReplication
{
    public interface IPrototypesStorage<TIdentifier>
    {
        void SetProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProvider);
        void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new();
    }
}