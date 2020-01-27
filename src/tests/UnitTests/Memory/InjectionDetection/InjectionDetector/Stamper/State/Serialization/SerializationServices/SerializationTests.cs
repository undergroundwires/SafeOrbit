using System;
using System.IO;
using NUnit.Framework;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices
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

        private static SharpSerializer GetSut(BinarySettings settings) => new SharpSerializer(settings);

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