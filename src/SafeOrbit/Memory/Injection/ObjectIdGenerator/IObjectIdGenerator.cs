namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    /// Abstracts a service that generates state ID's for any <see cref="object"/>.
    /// </summary>
    /// <seealso cref="IInjectionDetector"/>
    public interface IObjectIdGenerator
    {
        long GetStateId(object obj);
    }
}