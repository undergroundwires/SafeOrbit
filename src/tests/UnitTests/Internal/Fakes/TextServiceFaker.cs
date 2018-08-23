
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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