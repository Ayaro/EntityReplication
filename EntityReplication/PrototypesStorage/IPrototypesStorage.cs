using System;
using System.Linq.Expressions;
using EntityReplication.ExpressionParsing;

namespace EntityReplication.PrototypesStorage
{
    internal interface IPrototypesStorage<TIdentifier>
    {
        void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new();
        IExpressionContainer GetStoredPrototype(Type type);
    }
}