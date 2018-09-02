using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Abstraction for the service that stamps objects.
    /// </summary>
    /// <typeparam name="TObject">Type of object.</typeparam>
    /// <typeparam name="TStamp">The type of the the stamp.</typeparam>
    internal interface IStamper<in TObject, out TStamp>
    {
        InjectionType InjectionType { get; }
        TStamp GetStamp(TObject @object);
    }

    /// <summary>
    ///     Abstraction for the service that stamps objects with an <see cref="IStamp{THash}" /> stamp.
    /// </summary>
    /// <typeparam name="TObject">The type of the t object.</typeparam>
    internal interface IStamper<in TObject> : IStamper<TObject, IStamp<int>>
    {
    }
}