using System.Collections.Generic;
using System.Linq;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;
using SafeOrbit.Text;

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
                    async (c, e) => chars.AddRange(System.Text.Encoding.Unicode.GetString((await c.ToByteArrayAsync()))));
            fake.Setup(x => x.ToSafeBytesAsync())
                .Returns(async () =>
                {
                    var safeBytes = Stubs.Get<ISafeBytes>();
                    var bytes = System.Text.Encoding.Unicode.GetBytes(new string(chars.ToArray()));
                    foreach (var @byte in bytes)
                        await safeBytes.AppendAsync(@byte);
                    return safeBytes;
                });
            fake.Setup(x => x.GetAsSafeBytes(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    var c = chars.ElementAt(i);
                    var safeBytes = Stubs.Get<ISafeBytes>();
                    var bytes = System.Text.Encoding.Unicode.GetBytes($"{c}");
                    foreach (var @byte in bytes)
                        safeBytes.AppendAsync(@byte);
                    return safeBytes;
                });
            fake.Setup(x => x.GetAsCharAsync(It.IsAny<int>()))
                .ReturnsAsync((int i) => chars.ElementAt(i));
            fake.Setup(x => x.Length)
                .Returns(() => chars.Count());
            fake.Setup(x => x.IsEmpty)
                .Returns(() => !chars.Any());
            fake.Setup(x => x.DeepCloneAsync())
                .ReturnsAsync(() => fake.Object);
            fake.Setup(x => x.Equals(It.IsAny<string>()))
                .Returns((string text) => chars.ToArray().SequenceEqual(text.ToCharArray()));
            fake.Setup(x => x.IsDisposed)
                .Returns(() => isDisposed);
            fake.Setup(x => x.Dispose())
                .Callback(() => isDisposed = true);
            return fake.Object;
        }
    }
}