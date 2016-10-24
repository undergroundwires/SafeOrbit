namespace SafeOrbit.Memory
{
    public class SafeObjectFactory : ISafeObjectFactory
    {
        public static ISafeObjectFactory StaticInstance => new SafeObjectFactory();
        public ISafeObject<T> Get<T>(IInitialSafeObjectSettings settings) where T : class, new()
        {
            return new SafeObject<T>(settings);
        }
    }
    public interface ISafeObjectFactory
    {
        ISafeObject<T> Get<T>(IInitialSafeObjectSettings settings) where T : class, new();
    }
}