using Moq;
using SafeOrbit.Text;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    public class TextServiceFaker : StubProviderBase<ITextService>
    {
        public override ITextService Provide()
        {
            var fake = new Mock<ITextService>();
            var safeStringFactory = Stubs.GetFactory<ISafeString>();
            var encoding = System.Text.Encoding.Unicode;
            fake.Setup(x => x.GetBytes(It.IsAny<char>(), It.IsAny<Encoding>())).Returns(
                (char ch, Encoding e) => encoding.GetBytes(new[] { ch }));
            fake.Setup(x => x.GetBytes(It.IsAny<string>(), It.IsAny<Encoding>())).Returns(
                (string text, Encoding e) => encoding.GetBytes(text));
            fake.Setup(x => x.GetChars(It.IsAny<byte[]>(), It.IsAny<Encoding>())).Returns(
                (byte[] bytes, Encoding e) => encoding.GetChars(bytes));
            fake.Setup(x => x.Convert(It.IsAny<Encoding>(), It.IsAny<Encoding>(), It.IsAny<byte[]>())).Returns(
                (Encoding src, Encoding desc, byte[] bytes) => bytes);
            fake.Setup(x => x.GetString(It.IsAny<byte[]>(), It.IsAny<Encoding>())).Returns(
                (byte[] bytes, Encoding e) => encoding.GetString(bytes));
            fake.Setup(x => x.GetSafeString(It.IsAny<byte[]>(), It.IsAny<Encoding>())).Returns(
                (byte[] bytes, Encoding e) =>
                {
                    var result = safeStringFactory.Create();
                    var chars = encoding.GetChars(bytes);
                    foreach (var ch in chars)
                    {
                        result.Append(ch);
                    }
                    return result;
                });
            return fake.Object;
        }
    }
}