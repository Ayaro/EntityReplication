using System;
using System.Linq.Expressions;
using EntityReplication.DefaultValues;
using EntityReplication.PrototypesStorage;

namespace EntityReplication
{
    public class Replicator<TIdentifier>
    {
        private readonly IDefaultValuesFactory<TIdentifier> _defaultValuesFactory;
        private readonly IPrototypesStorage<TIdentifier> _prototypesStorage;
        private readonly ICreationalMethodBuilder<TIdentifier> _creationalMethodBuilder;

        public Replicator()
            : this(new DefaultValuesFactory<TIdentifier>())
        {
        }

        internal Replicator(IDefaultValuesFactory<TIdentifier> defaultValuesFactory)
            : this(defaultValuesFactory, new PrototypesStorage<TIdentifier>(defaultValuesFactory))
        {
        }

        internal Replicator(IDefaultValuesFactory<TIdentifier> defaultValuesFactory, IPrototypesStorage<TIdentifier> prototypesStorage)
            : this(defaultValuesFactory, prototypesStorage, new CreationalMethodBuilder<TIdentifier>(prototypesStorage))
        {
        }

        internal Replicator(IDefaultValuesFactory<TIdentifier> defaultValuesFactory, IPrototypesStorage<TIdentifier> prototypesStorage, ICreationalMethodBuilder<TIdentifier> creationalMethodBuilder)
        {
            _defaultValuesFactory = defaultValuesFactory;
            _prototypesStorage = prototypesStorage;
            _creationalMethodBuilder = creationalMethodBuilder;
        }

        public void SetProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProviderDelegate)
        {
            _defaultValuesFactory.SetDefaultValueProvider(defaultValueProviderDelegate);
        }

        public void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new()
        {
            _prototypesStorage.SetPrototype(prototypeExpression);
        }

        public TEntity Produce<TEntity>(TIdentifier id, Expression<Func<TEntity>> creationExpression = null)
            where TEntity : new()
        {
            return _creationalMethodBuilder.Produce(id, creationExpression);
        }
    }
}
