using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication.Tests
{
    [TestFixture]
    public class DefaultValueExpressionBuilderTests
    {
        private ParameterExpression _parameterExpression;
        private PropertyInfo _propertyInfo;

        private DefaultValueExpressionBuilder _defaultValueExpressionBuilder;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _parameterExpression = Expression.Parameter(typeof(int));
            _propertyInfo = typeof(Stub).GetProperties().First();

            _defaultValueExpressionBuilder = new DefaultValueExpressionBuilder(_parameterExpression, _propertyInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionIfParameterExpressionIsNull()
        {
            new DefaultValueExpressionBuilder(null, _propertyInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionIfPropertyInfoIsNull()
        {
            new DefaultValueExpressionBuilder(_parameterExpression, null);
        }

        [Test]
        public void ShouldBuildTypeDefaultValueExpression()
        {
            Expression expression = _defaultValueExpressionBuilder.BuildExpression(null);

            var defaultExpression = assertIsNotNullAndIsInstanceOf<DefaultExpression>(expression);
            Assert.AreEqual(_propertyInfo.PropertyType, defaultExpression.Type);
        }

        [Test]
        public void ShouldBuildInstanceMethodCallExpression()
        {
            var stub = new Stub();
            DefaultValueProviderDelegate<int, int> functor = stub.InstanceMethod;
            Expression expression = _defaultValueExpressionBuilder.BuildExpression(functor);

            var methodCallExpression = assertIsNotNullAndIsInstanceOf<MethodCallExpression>(expression);
            Assert.AreEqual(functor.Method, methodCallExpression.Method);

            var targetExpression = assertIsNotNullAndIsInstanceOf<ConstantExpression>(methodCallExpression.Object);
            Assert.AreEqual(stub.GetType(), targetExpression.Type);
            Assert.AreEqual(stub, targetExpression.Value);
        }

        [Test]
        public void ShouldBuildStaticMethodCallExpression()
        {
            DefaultValueProviderDelegate<int, int> functor = Stub.StaticMethod;
            Expression expression = _defaultValueExpressionBuilder.BuildExpression(functor);

            var methodCallExpression = assertIsNotNullAndIsInstanceOf<MethodCallExpression>(expression);
            Assert.AreEqual(functor.Method, methodCallExpression.Method);
            Assert.IsNull(methodCallExpression.Object);
        }

        private static T assertIsNotNullAndIsInstanceOf<T>(Expression expression) where T : Expression
        {
            Assert.IsNotNull(expression);
            Assert.IsInstanceOf<T>(expression);

            return (T) expression;
        } 

        private class Stub
        {
            public int AutoProperty { get; set; }

            public int InstanceMethod(int arg0, PropertyInfo arg1)
            {
                return 0;
            }

            public static int StaticMethod(int arg0, PropertyInfo arg1)
            {
                return 0;
            } 
        }
    }
}
