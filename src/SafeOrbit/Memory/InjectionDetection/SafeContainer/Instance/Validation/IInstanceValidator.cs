using System.Collections.Generic;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    /// <summary>
    ///     Abstracts a service that validates a <see cref="IReadOnlyList{T}" />
    /// </summary>
    /// <seealso cref="IInstanceProvider" />
    internal interface IInstanceValidator
    {
        void ValidateAll(IReadOnlyList<IInstanceProvider> instanceProviders);
    }
}