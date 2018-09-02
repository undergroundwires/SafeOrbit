namespace SafeOrbit.Tests
{
    public abstract class TestsFor<T> : TestsBase where T : class
    {
        protected abstract T GetSut();
    }
}
