using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization
{
    /// <seealso cref="Serializer" />
    /// <seealso cref="ISerializer" />
    [TestFixture]
    public class SerializerTests : TestsFor<ISerializer>
    {
        protected override ISerializer GetSut()
        {
            return new Serializer();
        }

        private void TestSerializationFor<TClass, TProperty>
            (Expression<Func<TClass, TProperty>> targetProperty, object value)
            where TClass : new()
        {
            // Arrange
            var sut = GetSut();
            var expected = value;
            var testClass = new TClass();
            var property = (targetProperty.Body as MemberExpression)?.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("Target property could not be found");
            property.SetValue(testClass, value, null);

            // Act
            var serialized = sut.Serialize(testClass);
            var deserialized = sut.Deserialize<TClass>(serialized);

            // Assert
            var actual = property.GetValue(deserialized);
            Assert.That(expected, Is.EqualTo(actual));
        }


        [Test]
        public void ArrayOfInts_CanSerialize_And_CanDeserialize()
        {
            // Arrange
            var sut = GetSut();
            var expected = new[] {5, 10, 15, 20430124, 54};

            // Act
            var serialized = sut.Serialize(expected);
            var actual = sut.Deserialize<int[]>(serialized);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void BoolProperty_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, bool>
            (
                t => t.TestBool,
                true
            );
        }

        [Test]
        public void ByteArrayProperty_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, byte[]>
            (
                t => t.TestBytes,
                new byte[] {5, 10, 20, 30}
            );
        }

        [Test]
        public void DateTimeProperty_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, DateTime?>
            (
                t => t.TestDateTime,
                DateTime.Now
            );
        }

        [Test]
        public void Deserialize_CanRetrieveSerializedObject()
        {
            //arrange
            var expected = "test";
            var sut = GetSut();
            //act
            var serializedObject = sut.Serialize(expected);
            var actual = sut.Deserialize<string>(serializedObject);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void DeSerialize_IfParemeterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var nullObject = (byte[]) null;
            var sut = GetSut();

            // Act
            void Deserialization() => sut.Deserialize<object>(nullObject);

            // Assert
            Assert.That(Deserialization, Throws.ArgumentNullException);
        }

        [Test]
        public void Dictionary_CanBeSerialized_And_CanBeDeserialized()
        {
            // Arrange
            var sut = GetSut();
            var expected1 = new TestClass
            {
                TestBytes = new byte[] {5, 10},
                TestString = "test",
                TestInt = 5,
                TestDateTime = DateTime.MaxValue,
                TestClassInstance = new TestClass {TestBytes = new byte[] {8, 8}, TestString = "bb", TestInt = 4}
            };
            var expected2 = new TestClass
            {
                TestBytes = new byte[] {20, 5},
                TestString = "test2",
                TestInt = 35,
                TestDateTime = DateTime.MinValue,
                TestClassInstance = new TestClass {TestBytes = new byte[] {5, 5}, TestString = "aa", TestInt = 2}
            };
            var dictionary = new Dictionary<int, TestClass> {{0, expected1}, {1, expected2}};

            // Act
            var serialized = sut.Serialize(dictionary);
            var deserialized = sut.Deserialize<Dictionary<int, TestClass>>(serialized);

            // Assert
            var actual1 = deserialized[0];
            var actual2 = deserialized[1];
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void IntProperty_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, int>
            (
                t => t.TestInt,
                25
            );
        }

        [Test]
        public void ListOfInts_CanSerialize_And_CanDeserialize()
        {
            // Arrange
            var sut = GetSut();
            var expected = new List<int> {55};

            // Act
            var serialized = sut.Serialize(expected);
            var actual = sut.Deserialize<IEnumerable<int>>(serialized);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void NestedClasses_CanSerialize_And_CanDeserialize()
        {
            var testClass = new TestClass
            {
                TestBytes = new byte[] {5, 10},
                TestString = "test",
                TestInt = 5,
                TestDateTime = DateTime.MaxValue,
                TestBool = true,
                TestClassInstance = new TestClass
                {
                    TestBytes = new byte[] {6, 7},
                    TestString = "test2",
                    TestInt = 51,
                    TestDateTime = DateTime.Today,
                    TestClassInstance = new TestClass
                    {
                        TestBytes = new byte[] {5, 88},
                        TestString = "test3",
                        TestInt = 52,
                        TestDateTime = DateTime.UtcNow,
                        TestBool = true
                    }
                }
            };
            TestSerializationFor<TestClass, TestClass>
            (
                t => t.TestClassInstance,
                testClass
            );
        }

        [Test]
        public void NullablePropertyWithNullValue_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, DateTime?>
            (
                t => t.TestDateTime,
                null
            );
        }

        [Test]
        public void Serialize_IfParemeterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var nullObject = (object) null;
            var sut = GetSut();

            // Act
            void Serialization() => sut.Serialize(nullObject);

            // Assert
            Assert.That(Serialization, Throws.ArgumentNullException);
        }

        [Test]
        public void StringProperty_CanSerialize_And_CanDeserialize()
        {
            TestSerializationFor<TestClass, string>
            (
                t => t.TestString,
                "abc"
            );
        }

        public class TestClass : IEquatable<TestClass>
        {
            public TestClass TestClassInstance { get; set; }
            public string TestString { get; set; }
            public byte[] TestBytes { get; set; }
            public int TestInt { get; set; }
            public DateTime? TestDateTime { get; set; }
            public bool TestBool { get; set; }

            public bool Equals(TestClass other)
            {
                return TestString == other.TestString && TestBytes == null && other.TestBytes == null ||
                       TestBytes != null && other.TestBytes != null &&
                       TestBytes.SequenceEqual(other.TestBytes)
                       && TestInt == other.TestInt
                       && TestDateTime == other.TestDateTime && TestClassInstance == null &&
                       other.TestClassInstance == null ||
                       TestClassInstance != null && other.TestClassInstance != null &&
                       TestClassInstance.Equals(other.TestClassInstance)
                       && TestBool == other.TestBool;
            }
        }
    }
}