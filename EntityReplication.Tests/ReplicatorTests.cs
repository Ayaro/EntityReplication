using NUnit.Framework;

namespace EntityReplication.Tests
{
    [TestFixture]
    public class ReplicatorTests
    {
        private const int Standard = 42;

        private Replicator<int> _replicator;

        [SetUp]
        public void SetUp()
        {
            _replicator = new Replicator<int>();
        }

        [Test]
        public void ShouldReplicatePropertyWithRequiredValue()
        {
            TestClass result = _replicator.Produce(0, () => new TestClass { Property = Standard });

            Assert.IsNotNull(result);
            Assert.AreEqual(Standard, result.Property);
        }

        [Test]
        public void ShouldReplicatePropertyFromPrototype()
        {
            _replicator.SetPrototype(id => new TestClass { Property = id });
            TestClass result = _replicator.Produce(Standard, () => new TestClass());

            Assert.IsNotNull(result);
            Assert.AreEqual(Standard, result.Property);
        }

        [Test]
        public void ShouldReplicatePropertyWithDefaultValue()
        {
            _replicator.SetProvider((id, propertyInfo) => id);
            TestClass result = _replicator.Produce(Standard, () => new TestClass());

            Assert.IsNotNull(result);
            Assert.AreEqual(Standard, result.Property);
        }

        [Test]
        public void ShouldReplicatePropertyWithoutUsingExpression()
        {
            _replicator.SetProvider((id, propertyInfo) => id);
            TestClass result = _replicator.Produce<TestClass>(Standard);

            Assert.IsNotNull(result);
            Assert.AreEqual(Standard, result.Property);
        }

        [Test]
        public void ShouldHaveDefaultTypeValueIfNotSpecified()
        {
            TestClass result = _replicator.Produce<TestClass>(Standard);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Property);
        }

        private class TestClass
        {
            public int Property { get; set; }
        }
    }
}
