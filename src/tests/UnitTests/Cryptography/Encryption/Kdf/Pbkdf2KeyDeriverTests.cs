using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption.Kdf
{
    /// <seealso cref="IKeyDerivationFunction" />
    /// <seealso cref="Pbkdf2KeyDeriver" />
    public class Pbkdf2KeyDeriverTests : TestsFor<IKeyDerivationFunction>
    {
        protected override IKeyDerivationFunction GetSut() => new Pbkdf2KeyDeriver();

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Derive_ForDifferentInstances_DerivationIsSame(byte[] key, byte[] salt)
        {
            //arrange
            var sut1 = GetSut();
            var sut2 = GetSut();
            var sut3 = GetSut();
            var sut4 = GetSut();
            var sut5 = GetSut();
            var length = key.Length;
            var expected = sut1.Derive(key, salt, length);
            //act
            var actual1 = sut2.Derive(key, salt, length);
            var actual2 = sut3.Derive(key, salt, length);
            var actual3 = sut4.Derive(key, salt, length);
            var actual4 = sut5.Derive(key, salt, length);
            //assert
            Assert.That(expected, Is.EqualTo(actual1));
            Assert.That(expected, Is.EqualTo(actual2));
            Assert.That(expected, Is.EqualTo(actual3));
            Assert.That(expected, Is.EqualTo(actual4));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Derive_WhenLengthIsLongerThanKey_CanDerive(byte[] key, byte[] salt)
        {
            //arrange
            var sut = GetSut();
            var size = key.Length*2;
            byte[] result = null;
            //act
            TestDelegate derivation = () => result = sut.Derive(key, salt, size);
            //assert
            Assert.DoesNotThrow(derivation);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Has.Length.EqualTo(size));
        }
    }
}