using EntityReplication.DefaultValues;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication.Tests
{
    [TestFixture]
    public class DefaultValuesFactoryTests
    {
        private ParameterExpression _parameterExpression;
        private PropertyInfo _propertyInfo;

        private DefaultValuesFactory<int> _defaultValuesFactory;

        [SetUp]
        public void SetUp()
        {
            _parameterExpression = Expression.Parameter(typeof (int));
            _propertyInfo = typeof (Stub).GetProperties().First();

            _defaultValuesFactory = new DefaultValuesFactory<int>();
        }

        [Test]
        public void ShouldReturnTypeDefaultValueBinding()
        {
            MemberBinding memberBinding = _defaultValuesFactory.DefaultValueBinding(_parameterExpression, _propertyInfo);

            assertMemberAssignmentExpressionIsInstanceOf<DefaultExpression>(memberBinding);
        }

        [Test]
        public void ShouldReturnMethodCallBinding()
        {
            _defaultValuesFactory.SetDefaultValueProvider((id, info) => 0);
            MemberBinding memberBinding = _defaultValuesFactory.DefaultValueBinding(_parameterExpression, _propertyInfo);

            assertMemberAssignmentExpressionIsInstanceOf<MethodCallExpression>(memberBinding);
        }

        private static void assertMemberAssignmentExpressionIsInstanceOf<T>(MemberBinding memberBinding)
        {
            Assert.IsNotNull(memberBinding);
            Assert.IsInstanceOf<MemberAssignment>(memberBinding);

            var memberAssignment = (MemberAssignment)memberBinding;
            Assert.IsInstanceOf<T>(memberAssignment.Expression);
        }

        private class Stub
        {
            public int AutoProperty { get; set; }
        }
    }
}
