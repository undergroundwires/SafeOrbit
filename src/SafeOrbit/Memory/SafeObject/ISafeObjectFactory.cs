namespace SafeOrbit.Memory
{
    public interface ISafeObjectFactory
    {
        ISafeObject<T> Get<T>(IInitialSafeObjectSettings settings) where T : class, new();
    }
}