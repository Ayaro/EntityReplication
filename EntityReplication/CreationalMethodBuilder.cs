using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityReplication.ExpressionParsing;
using EntityReplication.PrototypesStorage;

namespace EntityReplication
{
    internal class CreationalMethodBuilder<TIdentifier> : ICreationalMethodBuilder<TIdentifier>
    {
        private readonly IPrototypesStorage<TIdentifier> _prototypesStorage;

        public CreationalMethodBuilder(IPrototypesStorage<TIdentifier> prototypesStorage)
        {
            _prototypesStorage = prototypesStorage;
        }

        public TEntity Produce<TEntity>(TIdentifier id, Expression<Func<TEntity>> creationExpression = null)
            where TEntity : new()
        {
            IExpressionContainer prototypeContainer = _prototypesStorage.GetStoredPrototype(typeof(TEntity));
            IExpressionContainer requestedEntityContainer = ExpressionContainerFactory.Parse(creationExpression);
            Func<TIdentifier, TEntity> creationalMethod = buildCreationalMethod<TEntity>(prototypeContainer, requestedEntityContainer);

            return creationalMethod(id);
        }

        private static Func<TIdentifier, TEntity> buildCreationalMethod<TEntity>(IExpressionContainer prototypeContainer, IExpressionContainer requestedEntityContainer)
            where TEntity : new()
        {
            IEnumerable<MemberBinding> memberBindings = requestedEntityContainer.MemberBindings.Union(prototypeContainer.MemberBindings, new SingleBindingPerPropertyComparer());

            MemberInitExpression memberInitExpression = Expression.MemberInit(prototypeContainer.ConstructorExpression, memberBindings);
            Expression<Func<TIdentifier, TEntity>> lambdaExpression = Expression.Lambda<Func<TIdentifier, TEntity>>(memberInitExpression, prototypeContainer.ParameterExpression);

            return lambdaExpression.Compile();
        }
    }
}