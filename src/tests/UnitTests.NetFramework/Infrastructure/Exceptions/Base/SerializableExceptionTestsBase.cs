using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using SafeOrbit.Exceptions.SerializableException;
using SafeOrbit.Extensions;
using SafeOrbit.Tests;

namespace SafeOrbit.Exceptions
{
    [TestFixture]
    public abstract class SerializableExceptionTestsBase<T> : TestsFor<SerializableExceptionBase>
        where T : SerializableExceptionBase
    {
        protected abstract T GetSutForSerialization();
        protected virtual IEnumerable<PropertyInfo> GetExpectedPropertiesForSerialization() => null;

        protected SerializableExceptionBase GetSut(params object[] parameters)
            => parameters == null ? Activator.CreateInstance<T>() : (T) Activator.CreateInstance(typeof(T), parameters);

        protected override SerializableExceptionBase GetSut() => GetSut(null);



        [Serializable]
        private class SerializableExceptionTestClass : SerializableExceptionBase
        {
            public SerializableExceptionTestClass()
            {
            }

            public SerializableExceptionTestClass(string message) : base(message)
            {
            }

            public SerializableExceptionTestClass(string message, Exception inner) : base(message, inner)
            {
            }

            protected SerializableExceptionTestClass(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }


        //Serialization
        [Test]
        public void CanSerialize_And_Deserialize()
        {
            //arrange
            var sut = GetSutForSerialization();
            if (sut == null)
                throw new ArgumentException($"Please provide a sut by overriding {nameof(GetSutForSerialization)}");
            var expected = sut;
            //act
            T actual;
            using (var mStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(mStream, expected);
                mStream.Position = 0;
                actual = (T) formatter.Deserialize(mStream);
            }
            //assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<T>());
            Assert.That(actual.ResourceReferenceProperty, Is.EqualTo(expected.ResourceReferenceProperty));
            if (expected.InnerException != null)
                Assert.That(actual.InnerException?.Message, Is.EqualTo(expected.InnerException?.Message));
            else
                Assert.That(actual.InnerException, Is.Null);
            Assert.That(actual.Message, Is.EqualTo(expected.Message));
            var properties = GetExpectedPropertiesForSerialization().EmptyIfNull().ToArray();
            foreach (var property in properties)
            {
                var expectedValue = property.GetValue(expected);
                var actualValue = property.GetValue(actual);
                Assert.That(actualValue, Is.EqualTo(expectedValue));
            }
        }

        [Test]
        public void Constructor_Default_FromDefaultConstructor_SetsDefaultMessage()
        {
            if (!HasConstructor(Type.EmptyTypes)) IgnoreTest();
            //arrange
            var sut = GetSut();
            var expected = $"Exception of type '{sut.GetType().FullName}' was thrown.";
            //act
            var actual = sut.Message;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_Default_HasNoInnerException()
        {
            if (!HasConstructor(Type.EmptyTypes)) IgnoreTest();
            //arrange
            var sut = GetSut();
            var expected = Is.Null;
            //act
            var actual = sut.InnerException;
            //assert
            Assert.That(actual, expected);
        }


        [Test]
        public void Constructor_Default_HasNoResourceReferenceProperty()
        {
            if (!HasConstructor(Type.EmptyTypes)) IgnoreTest();
            //arrange
            var sut = GetSut();
            var expected = Is.Null;
            //act
            var actual = sut.ResourceReferenceProperty;
            //assert
            Assert.That(actual, expected);
        }

        [Test]
        public void Constructor_InnerException_HasNoResourceReferenceProperty()
        {
            if (!HasConstructor(typeof(Exception))) IgnoreTest();
            //arrange
            var exception = new Exception("foo");
            var sut = GetSut(exception);
            var expected = Is.Null;
            //act
            var actual = sut.ResourceReferenceProperty;
            //assert
            Assert.That(actual, expected);
        }

        [Test]
        public void Constructor_InnerException_SetsInnerException()
        {
            if (!HasConstructor(typeof(Exception))) IgnoreTest();
            //arrange
            var expected = new Exception("foo");
            var sut = GetSut(expected);
            //act
            var actual = sut.InnerException;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_InnerException_SetsMessage()
        {
            if (!HasConstructor(typeof(Exception))) IgnoreTest();
            //arrange
            var expected =
                $"InnerException has occured. Check {nameof(SerializableExceptionBase.InnerException)} property";
            var exception = new Exception("foo");
            var sut = GetSut(exception);
            //act
            var actual = sut.Message;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_String_HasNoInnerException()
        {
            if (!HasConstructor(typeof(string))) IgnoreTest();
            //arrange
            const string @string = "string parameter";
            var sut = GetSut(@string);
            var expected = Is.Null;
            //act
            var actual = sut.InnerException;
            //assert
            Assert.That(actual, expected);
        }

        [Test]
        public void Constructor_String_HasNoResourceReferenceProperty()
        {
            if (!HasConstructor(typeof(string))) IgnoreTest();
            //arrange
            const string @string = "string parameter";
            var sut = GetSut(@string);
            var expected = Is.Null;
            //act
            var actual = sut.ResourceReferenceProperty;
            //assert
            Assert.That(actual, expected);
        }

        [Test]
        public void Constructor_String_SetsMessage()
        {
            if (!HasConstructor(typeof(string))) IgnoreTest();
            //arrange
            const string expected = "string parameter";
            var sut = GetSut(expected);
            //act
            var actual = sut.Message;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_StringAndEx_HasNoResourceReferenceProperty()
        {
            if (!HasConstructor(typeof(string), typeof(Exception))) IgnoreTest();
            //arrange
            const string @string = "string parameter";
            var exception = new Exception("Test exception");
            var sut = GetSut(@string, exception);
            var expected = Is.Null;
            //act
            var actual = sut.ResourceReferenceProperty;
            //assert
            Assert.That(actual, expected);
        }

        [Test]
        public void Constructor_StringAndEx_SetsInnerException()
        {
            if (!HasConstructor(typeof(string), typeof(Exception))) IgnoreTest();
            //arrange
            const string @string = "string parameter";
            var expected = new Exception("Expected exception");
            var sut = GetSut(@string, expected);
            //act
            var actual = sut.InnerException;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_StringAndEx_SetsMessage()
        {
            if (!HasConstructor(typeof(string), typeof(Exception))) IgnoreTest();
            //arrange
            const string expected = "string parameter";
            var exception = new Exception("Test exception");
            var sut = GetSut(expected, exception);
            //act
            var actual = sut.Message;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }


        //GetObjectData
        [Test]
        public void GetObjectData_ParameterIsNull_throwsArgumentNUllException()
        {
            // Arrange
            var sut = GetSutForSerialization();
            // Act
            TestDelegate invokingWithNullParamater = () => sut.GetObjectData(null, new StreamingContext());
            // Assert
            Assert.Throws<ArgumentNullException>(invokingWithNullParamater);
        }

        [Test]
        public void Sut_Derives_SafeOrbitException()
        {
            const bool expected = true;
            var expectedType = typeof(SafeOrbitException);
            var actualType = typeof(T);
            var actual = expectedType.IsAssignableFrom(actualType);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Sut_Has_Serializable_Attribute()
        {
            const bool expected = true;
            var actual = typeof(T).GetCustomAttributes(typeof(SerializableAttribute), true).Length == 1;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Sut_Has_Serialization_Constructor()
        {
            //arrange
            const bool expected = true;
            var ctorParamTypes = new[] {typeof(SerializationInfo), typeof(StreamingContext)};
            var actual =
                typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null,
                    ctorParamTypes, null) != null;
            Assert.That(actual, Is.EqualTo(expected));
        }
        private bool HasConstructor(params Type[] parameterTypes)
        {
            if (parameterTypes == null) throw new ArgumentNullException(nameof(parameterTypes));
            var constructor = typeof(T).GetConstructor(parameterTypes);
            return constructor != null;
        }

        protected PropertyInfo GetPropertyFromExpression<TObject>(Expression<Func<T, TObject>> getPropertyLambda)
        {
            MemberExpression exp = null;
            //this line is necessary, because sometimes the expression comes in as Convert(original expression)
            if (getPropertyLambda.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)getPropertyLambda.Body;
                if (UnExp.Operand is MemberExpression)
                    exp = (MemberExpression)UnExp.Operand;
                else
                    throw new ArgumentException();
            }
            else if (getPropertyLambda.Body is MemberExpression)
            {
                exp = (MemberExpression)getPropertyLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)exp.Member;
        }
    }
}