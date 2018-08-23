
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

using System;
using System.Collections.Generic;
using Moq;
using SafeOrbit.Memory.Common;
using SafeOrbit.Infrastructure.Serialization;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISerializer" />
    /// <seealso cref="Serializer" />
    public class SerializerFaker : StubProviderBase<ISerializer>
    {
        public override ISerializer Provide()
        {
            var fake = new Mock<ISerializer>();
            var innerValues = new Dictionary<byte[], object>();
            fake.Setup(s => s.Serialize(It.IsAny<object>())).Returns((object obj) =>
            {
                var serializedBytes = BitConverter.GetBytes(obj.GetHashCode()); //get bulk bytes
                var @object = Serializer.StaticInstance.Deserialize<object>(Serializer.StaticInstance.Serialize(obj)); // deeply clone
                if (!innerValues.ContainsKey(serializedBytes))
                    innerValues.Add(serializedBytes, @object);
                else
                    innerValues[serializedBytes] = @object;
                return serializedBytes; // return bulk bytes
            });
            fake.Setup(s => s.Deserialize<object>(It.IsAny<byte[]>())).Returns((byte[] bytes) =>
            {
                object value;
                innerValues.TryGetValue(bytes, out value);
                return value;
            });
            fake.Setup(s => s.Deserialize<IEnumerable<int>>(It.IsAny<byte[]>())).Returns((byte[] bytes) =>
            {
                object value;
                innerValues.TryGetValue(bytes, out value);
                return value as IEnumerable<int>;
            });
            return fake.Object;
        }
    }
}