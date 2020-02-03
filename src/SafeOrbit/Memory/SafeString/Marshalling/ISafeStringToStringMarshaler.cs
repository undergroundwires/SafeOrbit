using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeStringServices
{
    /// <summary>
    ///     A marshaler that can convert a <see cref="ISafeString" /> into a <see cref="string" /> until it is disposed
    /// </summary>
    internal interface ISafeStringToStringMarshaler
    {
        Task<IDisposableString> MarshalAsync(IReadOnlySafeString safeString);
    }
}