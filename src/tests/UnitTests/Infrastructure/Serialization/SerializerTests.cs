
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using SafeOrbit.Memory.Common;
using SafeOrbit.Infrastructure.Serialization;
using SafeOrbit.Tests;

namespace SafeOrbit.Infrastructure.Serialization
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
            //arrange
            var sut = GetSut();
            var expected = value;
            var testClass = new TClass();
            var property = (targetProperty.Body as MemberExpression)?.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("Target property could not be found");
            property.SetValue(testClass, value, null);
            //act
            var serialized = sut.Serialize(testClass);
            var deserialized = sut.Deserialize<TClass>(serialized);
            //assert
            var actual = property.GetValue(deserialized);
            Assert.That(expected, Is.EqualTo(actual));
        }

        

        [Test]
        public void ArrayOfInts_CanSerialize_And_CanDeserialize()
        {
            //arrange
            var sut = GetSut();
            var expected = new[] {5, 10, 15, 20430124, 54};
            //act
            var serialized = sut.Serialize(expected);
            var actual = sut.Deserialize<int[]>(serialized);
            //assert
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
            //arrange
            var nullObject = (byte[]) null;
            var sut = GetSut();
            //act
            TestDelegate deserialization = () => sut.Deserialize<object>(nullObject);
            //assert
            Assert.That(deserialization, Throws.ArgumentNullException);
        }

        [Test]
        public void Dictionary_CanBeSerialized_And_CanBeDeserialized()
        {
            //arrange
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
            var dictionary = new Dictionary<int, TestClass>();
            dictionary.Add(0, expected1);
            dictionary.Add(1, expected2);
            //act
            var serialized = sut.Serialize(dictionary);
            var deserialized = sut.Deserialize<Dictionary<int, TestClass>>(serialized);
            //assert
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
            //arrange
            var sut = GetSut();
            var expected = new List<int>();
            expected.Add(55);
            //act
            var serialized = sut.Serialize(expected);
            var actual = sut.Deserialize<IEnumerable<int>>(serialized);
            //assert
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
            //arrange
            var nullObject = (object) null;
            var sut = GetSut();
            //act
            TestDelegate serialization = () => sut.Serialize(nullObject);
            //assert
            Assert.That(serialization, Throws.ArgumentNullException);
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
                return ((TestString == other.TestString) && (TestBytes == null) && (other.TestBytes == null)) ||
                       ((TestBytes != null) && (other.TestBytes != null) &&
                        TestBytes.SequenceEqual(other.TestBytes)
                        && (TestInt == other.TestInt)
                        && (TestDateTime == other.TestDateTime) && (TestClassInstance == null) &&
                        (other.TestClassInstance == null)) ||
                       ((TestClassInstance != null) && (other.TestClassInstance != null) &&
                        TestClassInstance.Equals(other.TestClassInstance)
                        && (TestBool == other.TestBool));
            }
        }
    }
}