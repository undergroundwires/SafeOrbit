using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Tests.Equality;

namespace SafeOrbit.Memory.Injection
{
    /// <seealso cref="EqualityTestsBase{T}" />
    /// <seealso cref="InjectionMessage" />
    /// <seealso cref="IInjectionMessage" />
    [TestFixture]
    public class InjectionMessageTests : EqualityTestsBase<InjectionMessage>
    {
        protected override InjectionMessage GetSut() => new InjectionMessage(true, false, 5);

        [Test]
        public void ToString_StateInjected_PrintsInjectionType()
        {
            // Arrange
            var sut = new InjectionMessage(true, false, "");
            
            // Act
            var actual = sut.ToString();

            // Assert
            var expected = InjectionType.VariableInjection.ToString();
            Assert.That(actual, Does.Contain(expected));
        }

        [Test]
        public void ToString_CodeInjected_PrintsInjectionType()
        {
            // Arrange
            var sut = new InjectionMessage(false, true, "");

            // Act
            var actual = sut.ToString();

            // Assert
            var expected = InjectionType.CodeInjection.ToString();
            Assert.That(actual, Does.Contain(expected));
        }


        [Test]
        public void ToString_CodeAndStateInjected_PrintsInjectionType()
        {
            // Arrange
            var sut = new InjectionMessage(true, true, "");

            // Act
            var actual = sut.ToString();

            // Assert
            var expected = InjectionType.CodeAndVariableInjection.ToString();
            Assert.That(actual, Does.Contain(expected));
        }

        [Test]
        public void ToString_InjectionTime_IsPrinted()
        {
            // Arrange
            var sut = new InjectionMessage(true, true, "");

            // Act
            var actual = sut.ToString();

            // Assert
            var expected = sut.InjectionDetectionTime.ToString();
            Assert.That(actual, Does.Contain(expected));
        }


        [Test]
        public void ToString_ObjectType_IsPrinted()
        {
            // Arrange
            var injectedObject = string.Empty;
            var sut = new InjectionMessage(true, true, injectedObject);

            // Act
            var actual = sut.ToString();

            // Assert
            var expected = injectedObject.GetType().Name;
            Assert.That(actual, Does.Contain(expected));
        }

        protected override IEnumerable<InjectionMessage> GetDifferentSuts() => new[]
        {
            new InjectionMessage(false, true, 5),
            new InjectionMessage(true, true, 5),
            new InjectionMessage(true, false, 6),
            new InjectionMessage(true, false, "aq")
        };
    }
}