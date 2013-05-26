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
        private readonly DefaultValuesFactory<TIdentifier> _defaultValuesFactory = new DefaultValuesFactory<TIdentifier>();
        private readonly Dictionary<Type, ExpressionContainer> _prototypesStorage = new Dictionary<Type, ExpressionContainer>();

        public void SetProvider<TValue>(DefaultValueProviderDelegate<TIdentifier, TValue> defaultValueProvider)
        {
            _defaultValuesFactory.SetDefaultValueProvider(defaultValueProvider);
        }

        public void SetPrototype<TEntity>(Expression<Func<TIdentifier, TEntity>> prototypeExpression) where TEntity : new()
        {
            _prototypesStorage[typeof(TEntity)] = createPrototypeContainer(typeof(TEntity), prototypeExpression);
        }

        public TEntity Produce<TEntity>(TIdentifier id, Expression<Func<TEntity>> prototypeExpression = null) where TEntity : new()
        {
            ExpressionContainer prototypeContainer = getStoredPrototype(typeof(TEntity));
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

        private ExpressionContainer getStoredPrototype(Type type)
        {
            ExpressionContainer expressionContainer;

            if (!_prototypesStorage.TryGetValue(type, out expressionContainer))
            {
                expressionContainer = createPrototypeContainer(type, null);
                _prototypesStorage[type] = expressionContainer;
            }

            return expressionContainer;
        }

        private ExpressionContainer createPrototypeContainer(Type entityType, LambdaExpression prototypeExpression)
        {
            IExpressionContainer prototypeContainer = ExpressionContainerFactory.Create(prototypeExpression);

            ParameterExpression parameterExpression = prototypeContainer.ParameterExpression ?? Expression.Parameter(typeof(TIdentifier));
            NewExpression constructorExpression = prototypeContainer.ConstructorExpression ?? Expression.New(entityType);
            IEnumerable<MemberBinding> memberBindings = fillWithDefaultValueBindings(entityType, parameterExpression, prototypeContainer.MemberBindings);

            return new ExpressionContainer(parameterExpression, constructorExpression, memberBindings);
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
