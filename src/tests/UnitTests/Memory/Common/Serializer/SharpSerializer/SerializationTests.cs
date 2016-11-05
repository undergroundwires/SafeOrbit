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

using System;
using System.IO;
using NUnit.Framework;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices
{
    [TestFixture]
    public class SerializationTests
    {
        /// <summary>
        ///     Local testclass to be serialized
        /// </summary>
        public class ClassWithGuid
        {
            public Guid Guid { get; set; }
        }

        /// <summary>
        ///     Local testclass to be serialized
        /// </summary>
        public class ParentChildTestClass
        {
            public string Name { get; set; }
            public ParentChildTestClass Mother { get; set; }
            public ParentChildTestClass Father { get; set; }
        }
        private SharpSerializer GetSut(BinarySettings settings) => new SharpSerializer(settings);
        [Test]
        public void BinSerial_ShouldSerializeGuid()
        {
            var parent = new ClassWithGuid
            {
                Guid = Guid.NewGuid()
            };

            var stream = new MemoryStream();
            var settings = new BinarySettings(BinarySerializationMode.SizeOptimized);
            var serializer = GetSut(settings);

            serializer.Serialize(parent, stream);


            serializer = GetSut(settings);
            stream.Position = 0;
            var loaded = serializer.Deserialize(stream) as ClassWithGuid;

            Assert.AreEqual(parent.Guid, loaded.Guid, "same guid");
        }

        [Test]
        public void BinSerial_TwoIdenticalChildsShouldBeSameInstance()
        {
            var parent = new ParentChildTestClass
            {
                Name = "parent"
            };

            var child = new ParentChildTestClass
            {
                Name = "child",
                Father = parent,
                Mother = parent
            };

            Assert.AreSame(child.Father, child.Mother, "Precondition: Saved Father and Mother are same instance");

            var stream = new MemoryStream();
            var settings = new BinarySettings(BinarySerializationMode.SizeOptimized);
            var serializer = GetSut(settings);

            serializer.Serialize(child, stream);

            serializer = GetSut(settings);
            stream.Position = 0;
            var loaded = serializer.Deserialize(stream) as ParentChildTestClass;

            Assert.AreSame(loaded.Father, loaded.Mother, "Loaded Father and Mother are same instance");
        }
    }
}