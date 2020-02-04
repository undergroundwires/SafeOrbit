using System.Collections.Generic;
using System.Linq;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;
using SafeOrbit.Memory.SafeStringServices.Text;
using SafeOrbit.Threading;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeString" />
    public class SafeStringFaker : StubProviderBase<ISafeString>
    {
        public override ISafeString Provide()
        {
            var fake = new Mock<ISafeString>();
            var chars = new List<char>();
            var isDisposed = false;
            fake.Setup(x => x.AppendAsync(It.IsAny<char>()))
                .Callback<char>(x => chars.Add(x));
            fake.Setup(x => x.AppendAsync(It.IsAny<string>()))
                .Callback<string>(x => chars.AddRange(x));
            fake.Setup(x => x.AppendAsync(It.IsAny<ISafeBytes>(), It.IsAny<Encoding>()))
                .Callback<ISafeBytes, Encoding>(
                    (bytes, e) => chars.AddRange(System.Text.Encoding.Unicode.GetString(TaskContext.RunSync(bytes.RevealDecryptedBytesAsync))));
            fake.Setup(x => x.RevealDecryptedBytesAsync())
                .ReturnsAsync(() => System.Text.Encoding.Unicode.GetBytes(new string(chars.ToArray())));
            fake.Setup(x => x.GetAsSafeBytes(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    var c = chars.ElementAt(i);
                    var safeBytes = Stubs.Get<ISafeBytes>();
                    var bytes = System.Text.Encoding.Unicode.GetBytes($"{c}");
                    foreach (var @byte in bytes)
                        TaskContext.RunSync(() => safeBytes.AppendAsync(@byte));
                    return safeBytes;
                });
            fake.Setup(x => x.RevealDecryptedCharAsync(It.IsAny<int>()))
                .ReturnsAsync((int i) => chars.ElementAt(i));
            fake.Setup(x => x.Length)
                .Returns(() => chars.Count);
            fake.Setup(x => x.IsEmpty)
                .Returns(() => !chars.Any());
            fake.Setup(x => x.DeepCloneAsync())
                .ReturnsAsync(() => fake.Object);
            fake.Setup(x => x.EqualsAsync(It.IsAny<string>()))
                .ReturnsAsync((string text) => chars.AsEnumerable().SequenceEqual(text.ToCharArray()));
            fake.Setup(x => x.IsDisposed)
                .Returns(() => isDisposed);
            fake.Setup(x => x.Dispose())
                .Callback(() => isDisposed = true);
            return fake.Object;
        }
    }
}