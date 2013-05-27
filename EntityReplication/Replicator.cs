using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityReplication.ExpressionParsing;

namespace EntityReplication
{
    public class Replicator<TIdentifier>
    {
        private readonly PrototypesStorage<TIdentifier> _prototypesStorage = new PrototypesStorage<TIdentifier>();
        public IPrototypesStorage<TIdentifier> Storage { get { return _prototypesStorage; } }

        public TEntity Produce<TEntity>(TIdentifier id, Expression<Func<TEntity>> prototypeExpression = null) where TEntity : new()
        {
            IExpressionContainer prototypeContainer = _prototypesStorage.GetStoredPrototype(typeof(TEntity));
            IExpressionContainer requestedEntityContainer = ExpressionContainerFactory.Create(prototypeExpression);

            var memberBindings = new List<MemberBinding>(requestedEntityContainer.MemberBindings);
            var presentMembers = new HashSet<MemberInfo>(memberBindings.Select(x => x.Member));

            foreach (MemberBinding memberBinding in prototypeContainer.MemberBindings)
            {
                if (!presentMembers.Contains(memberBinding.Member))
                {
                    memberBindings.Add(memberBinding);
                }
            }

            var expression = Expression.MemberInit(prototypeContainer.ConstructorExpression, memberBindings);
            Expression<Func<TIdentifier, TEntity>> lambda = Expression.Lambda<Func<TIdentifier, TEntity>>(expression, prototypeContainer.ParameterExpression);
            Func<TIdentifier, TEntity> getter = lambda.Compile();

            return getter(id);
        }
    }
}
