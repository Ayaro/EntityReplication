using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityReplication.DefaultValues;
using EntityReplication.ExpressionParsing;

namespace EntityReplication.PrototypesStorage
{
    internal class PrototypesStorage<TIdentifier> : IPrototypesStorage<TIdentifier>
    {
        private readonly IDefaultValuesFactory<TIdentifier> _defaultValuesFactory;
        private readonly Dictionary<Type, IExpressionContainer> _prototypesStorage = new Dictionary<Type, IExpressionContainer>();

        public PrototypesStorage(IDefaultValuesFactory<TIdentifier> defaultValuesFactory)
        {
            _defaultValuesFactory = defaultValuesFactory;
        }

        public void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new()
        {
            _prototypesStorage[typeof(TEntity)] = createPrototypeContainer(typeof(TEntity), prototypeExpression);
        }

        public IExpressionContainer GetStoredPrototype(Type type)
        {
            IExpressionContainer expressionContainer;

            if (!_prototypesStorage.TryGetValue(type, out expressionContainer))
            {
                expressionContainer = createPrototypeContainer(type, null);
                _prototypesStorage[type] = expressionContainer;
            }

            return expressionContainer;
        }

        private IExpressionContainer createPrototypeContainer(Type entityType, LambdaExpression prototypeExpression)
        {
            IExpressionContainer prototypeContainer = ExpressionContainerFactory.Parse(prototypeExpression);

            return enhancePrototypeContainerWithDefaults(entityType, prototypeContainer);
        }

        private ExpressionContainer enhancePrototypeContainerWithDefaults(Type entityType, IExpressionContainer prototypeContainer)
        {
            ParameterExpression parameterExpression = prototypeContainer.ParameterExpression ?? Expression.Parameter(typeof (TIdentifier));
            NewExpression constructorExpression = prototypeContainer.ConstructorExpression ?? Expression.New(entityType);
            IEnumerable<MemberBinding> memberBindings = prototypeContainer.MemberBindings ?? Enumerable.Empty<MemberBinding>();
            IEnumerable<MemberBinding> defaultBindings = defaultValueBindings(entityType, parameterExpression, memberBindings);

            return new ExpressionContainer(parameterExpression, constructorExpression, memberBindings.Concat(defaultBindings));
        }

        private IEnumerable<MemberBinding> defaultValueBindings(Type entityType, ParameterExpression parameterExpression, IEnumerable<MemberBinding> prototypeBindings)
        {
            IEnumerable<MemberInfo> unbindedProperties = entityType.GetPropertiesExceptBinded(prototypeBindings);

            foreach (PropertyInfo propertyInfo in unbindedProperties)
            {
                yield return _defaultValuesFactory.DefaultValueBinding(parameterExpression, propertyInfo);
            }
        }
    }
}