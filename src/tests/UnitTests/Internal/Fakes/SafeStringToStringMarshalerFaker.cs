using System.Runtime.InteropServices;
using System.Text;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeStringServices;
using SafeOrbit.Tests;

namespace SafeOrbit.Internal.Fakes
{
    /// <seealso cref="ISafeStringToStringMarshaler" />
    internal class SafeStringToStringMarshalerFaker : StubProviderBase<ISafeStringToStringMarshaler>
    {
        public override ISafeStringToStringMarshaler Provide()
        {
            var fake = new Mock<ISafeStringToStringMarshaler>();
            fake.Setup(f => f.MarshalAsync(It.IsAny<IReadOnlySafeString>()))
                .Returns(async (IReadOnlySafeString str) =>
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < str.Length; i++)
                        sb.Append(await str.RevealDecryptedCharAsync(i));
                    var @string = sb.ToString();
                    var handle = GCHandle.Alloc(@string, GCHandleType.Pinned);
                    return new DisposableString(@string, handle);
                });
            return fake.Object;
        }
    }
}
