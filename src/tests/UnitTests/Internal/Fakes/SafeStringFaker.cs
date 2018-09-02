using System.Collections.Generic;
using System.Linq;
using Moq;
using SafeOrbit.Text;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

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
            fake.Setup(x => x.Append(It.IsAny<char>())).Callback<char>(x => chars.Add(x));
            fake.Setup(x => x.Append(It.IsAny<string>())).Callback<string>(x => chars.AddRange(x));
            fake.Setup(x => x.Append(It.IsAny<ISafeBytes>(), It.IsAny<Encoding>())).Callback<ISafeBytes, Encoding>(
                (c, e) => chars.Add((char)c.ToByteArray().FirstOrDefault())); //this  might broke new tests
            fake.Setup(x => x.ToSafeBytes()).Returns(() =>
            {
                var safeBytes = Stubs.Get<ISafeBytes>();
                foreach (char c in chars)
                {
                    safeBytes.Append((byte)c);
                }
                return safeBytes;
            });
            fake.Setup(x => x.GetAsSafeBytes(It.IsAny<int>())).Returns((int i) =>
            {
                var c = chars.ElementAt(i);
                var safeBytes = Stubs.Get<ISafeBytes>();
                safeBytes.Append((byte)c);
                return safeBytes;
            });
            fake.Setup(x => x.GetAsChar(It.IsAny<int>())).Returns((int i) => chars.ElementAt(i));
            fake.Setup(x => x.Length).Returns(() => chars.Count());
            fake.Setup(x => x.IsEmpty).Returns(() => !chars.Any());
            fake.Setup(x => x.DeepClone()).Returns(() => fake.Object);
            fake.Setup(x => x.Equals(It.IsAny<string>())).Returns((string text) => chars.ToArray().SequenceEqual(text.ToCharArray()));
            fake.Setup(x => x.IsDisposed).Returns(() => isDisposed);
            fake.Setup(x => x.Dispose()).Callback(() => isDisposed = true);
            return fake.Object;
        }
    }
}