namespace SafeOrbit.Tests
{
    public abstract class StubProviderBase<TComponent> : IStubProvider<TComponent>
    {
        public abstract TComponent Provide();
        object IStubProvider.Provide() => Provide();
    }
}