using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Threading;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeByteFactory" />
    internal class SafeByteFactoryFaker : StubProviderBase<ISafeByteFactory>
    {
        public override ISafeByteFactory Provide()
        {
            var fake = new Mock<ISafeByteFactory>();
            fake.Setup(x => x.GetByByteAsync(It.IsAny<byte>()))
                .ReturnsAsync((byte @byte) =>
                    {
                        var safeByte = Stubs.Get<ISafeByte>();
                        TaskContext.RunSync(() => safeByte.SetAsync(@byte));
                        return safeByte;
                    });
            fake.Setup(x => x.GetByBytesAsync(It.IsAny<SafeMemoryStream>()))
                .ReturnsAsync((SafeMemoryStream stream) =>
                    {
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);

                        var result = new Collection<ISafeByte>();
                        foreach (var @byte in bytes)
                        {
                            var safeByte = Stubs.Get<ISafeByte>();
                            TaskContext.RunSync(() => safeByte.SetAsync(@byte));
                            result.Add(safeByte);
                        }
                        return result;
                    });
            fake.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(
                (int id) =>
                    {
                        var safeByte = Stubs.Get<ISafeByte>();
                        TaskContext.RunSync(() => safeByte.SetAsync((byte)id));
                        return safeByte;
                    });
            fake.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync((IEnumerable<int> byteIds) =>
                {
                    var result = new Collection<ISafeByte>();
                    foreach (var byteId in byteIds)
                    {
                        var safeByte = Stubs.Get<ISafeByte>();
                        TaskContext.RunSync(() => safeByte.SetAsync((byte)byteId));
                        result.Add(safeByte);
                    }
                    return result.ToArray();
                });
            return fake.Object;
        }
    }
}