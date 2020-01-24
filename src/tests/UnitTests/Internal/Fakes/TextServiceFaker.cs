using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;
using SafeOrbit.Text;
using Encoding = System.Text.Encoding;

namespace SafeOrbit.Fakes
{
    public class TextServiceFaker : StubProviderBase<ITextService>
    {
        public override ITextService Provide()
        {
            var fake = new Mock<ITextService>();
            var safeStringFactory = Stubs.GetFactory<ISafeString>();
            var encoding = Encoding.Unicode;
            fake.Setup(x => x.GetBytes(It.IsAny<char>(), It.IsAny<Text.Encoding>())).Returns(
                (char ch, Text.Encoding e) => encoding.GetBytes(new[] {ch}));
            fake.Setup(x => x.GetBytes(It.IsAny<string>(), It.IsAny<Text.Encoding>())).Returns(
                (string text, Text.Encoding e) => encoding.GetBytes(text));
            fake.Setup(x => x.GetChars(It.IsAny<byte[]>(), It.IsAny<Text.Encoding>())).Returns(
                (byte[] bytes, Text.Encoding e) => encoding.GetChars(bytes));
            fake.Setup(x => x.Convert(It.IsAny<Text.Encoding>(), It.IsAny<Text.Encoding>(), It.IsAny<byte[]>()))
                .Returns(
                    (Text.Encoding src, Text.Encoding desc, byte[] bytes) => bytes);
            fake.Setup(x => x.GetString(It.IsAny<byte[]>(), It.IsAny<Text.Encoding>())).Returns(
                (byte[] bytes, Text.Encoding e) => encoding.GetString(bytes));
            fake.Setup(x => x.GetSafeStringAsync(It.IsAny<byte[]>(), It.IsAny<Text.Encoding>())).Returns(
                async (byte[] bytes, Text.Encoding e) =>
                {
                    var result = safeStringFactory.Create();
                    var chars = encoding.GetChars(bytes);
                    foreach (var ch in chars)
                        await result.AppendAsync(ch);
                    return result;
                });
            return fake.Object;
        }
    }
}