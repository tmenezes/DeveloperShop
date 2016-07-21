using NUnit.Framework;

namespace DeveloperShop.Tests
{
    [TestFixture]
    public abstract class AbstractTest
    {
        public abstract void Arrange();
        public abstract void Act();

        [SetUp]
        public void Init()
        {
            this.Arrange();
        }
    }

    [TestFixture]
    public abstract class AbstractTestAutoAct
    {
        public abstract void Arrange();
        public abstract void Act();

        [SetUp]
        public void Init()
        {
            this.Arrange();
            this.Act();
        }
    }
}