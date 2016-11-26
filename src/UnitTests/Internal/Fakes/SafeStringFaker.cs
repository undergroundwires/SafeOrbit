
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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