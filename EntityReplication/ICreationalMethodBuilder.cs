using System;
using System.Linq.Expressions;

namespace EntityReplication
{
    internal interface ICreationalMethodBuilder<TIdentifier>
    {
        TEntity Produce<TEntity>(TIdentifier id, Expression<Func<TEntity>> creationExpression = null) where TEntity : new();
    }
}