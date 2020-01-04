using System;
using System.Collections.Generic;
using Moq;
using SafeOrbit.Memory.Common;
using SafeOrbit.Core.Serialization;
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