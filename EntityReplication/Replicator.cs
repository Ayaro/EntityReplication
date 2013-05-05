using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication
{
    public class Replicator<TIdentifier>
    {
        private readonly DefaultValuesFactory<TIdentifier> _defaultValuesFactory = new DefaultValuesFactory<TIdentifier>();
        private readonly Dictionary<Type, MemberInitContainer> _prototypesStorage = new Dictionary<Type, MemberInitContainer>();

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
            MemberInitContainer prototypeContainer = getStoredPrototype(typeof (TEntity));
            MemberInitContainer requestedEntityContainer = parseExpression(prototypeExpression);

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

        private MemberInitContainer getStoredPrototype(Type type)
        {
            MemberInitContainer memberInitContainer;

            if (!_prototypesStorage.TryGetValue(type, out memberInitContainer))
            {
                memberInitContainer = createPrototypeContainer(type, null);
                _prototypesStorage[type] = memberInitContainer;
            }

            return memberInitContainer;
        }

        private MemberInitContainer createPrototypeContainer(Type entityType, LambdaExpression prototypeExpression)
        {
            MemberInitContainer prototypeContainer = parseExpression(prototypeExpression);

            ParameterExpression parameterExpression = prototypeContainer.ParameterExpression ?? Expression.Parameter(typeof(TIdentifier));
            NewExpression constructorExpression = prototypeContainer.ConstructorExpression ?? Expression.New(entityType);
            IEnumerable<MemberBinding> memberBindings = fillWithDefaultValueBindings(entityType, parameterExpression, prototypeContainer.MemberBindings);

            return new MemberInitContainer(parameterExpression, constructorExpression, memberBindings);
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

        private MemberInitContainer parseExpression(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null)
            {
                return new MemberInitContainer(null, null, Enumerable.Empty<MemberBinding>());
            }

            ParameterExpression parameterExpression = lambdaExpression.Parameters.FirstOrDefault();

            NewExpression constructorExpression;
            IEnumerable<MemberBinding> memberBindings;

            var memberInitExpression = lambdaExpression.Body as MemberInitExpression;
            if (memberInitExpression != null)
            {
                constructorExpression = memberInitExpression.NewExpression;
                memberBindings = memberInitExpression.Bindings;
            }
            else
            {
                constructorExpression = lambdaExpression.Body as NewExpression;
                memberBindings = Enumerable.Empty<MemberBinding>();
            }

            return new MemberInitContainer(parameterExpression, constructorExpression, memberBindings);
        }
    }
}
