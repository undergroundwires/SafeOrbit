using NUnit.Framework;

namespace SafeOrbit.Tests
{
    public abstract class TestsFor<T> : TestsBase where T : class
    {
        protected abstract T GetSut();

        [Test]
        public void Ctor_can_create_sut()
        {
            var sut = GetSut();
            Assert.NotNull(sut);
        }
    }
}