using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityReplication.ExpressionParsing;

namespace EntityReplication
{
    internal class PrototypesStorage<TIdentifier> : IPrototypesStorage<TIdentifier>
    {
        private readonly DefaultValuesFactory<TIdentifier> _defaultValuesFactory = new DefaultValuesFactory<TIdentifier>();
        private readonly Dictionary<Type, IExpressionContainer> _prototypesStorage = new Dictionary<Type, IExpressionContainer>();

        public void SetProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProvider)
        {
            _defaultValuesFactory.SetDefaultValueProvider(defaultValueProvider);
        }

        public void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new()
        {
            _prototypesStorage[typeof(TEntity)] = createPrototypeContainer(typeof(TEntity), prototypeExpression);
        }

        internal IExpressionContainer GetStoredPrototype(Type type)
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
            IExpressionContainer prototypeContainer = ExpressionContainerFactory.Create(prototypeExpression);

            return enhanceExpressionContainer(entityType, prototypeContainer);
        }

        private ExpressionContainer enhanceExpressionContainer(Type entityType, IExpressionContainer prototypeContainer)
        {
            ParameterExpression parameterExpression = prototypeContainer.ParameterExpression ?? Expression.Parameter(typeof (TIdentifier));
            NewExpression constructorExpression = prototypeContainer.ConstructorExpression ?? Expression.New(entityType);
            IEnumerable<MemberBinding> memberBindings = prototypeContainer.MemberBindings ?? Enumerable.Empty<MemberBinding>();
            IEnumerable<MemberBinding> enhancedMemberBindings = fillWithDefaultValueBindings(entityType, parameterExpression, memberBindings);

            return new ExpressionContainer(parameterExpression, constructorExpression, enhancedMemberBindings);
        }

        private IEnumerable<MemberBinding> fillWithDefaultValueBindings(Type entityType, ParameterExpression parameterExpression, IEnumerable<MemberBinding> prototypeBindings)
        {
            var memberBindings = new List<MemberBinding>(prototypeBindings);
            var prototypeProperties = new HashSet<MemberInfo>(memberBindings.Select(x => x.Member));

            PropertyInfo[] properties = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in properties.Where(x => !prototypeProperties.Contains(x)))
            {
                MemberBinding defaultValueBinding = _defaultValuesFactory.DefaultValueBinding(parameterExpression, propertyInfo);
                memberBindings.Add(defaultValueBinding);
            }

            return memberBindings;
        }
    }
}