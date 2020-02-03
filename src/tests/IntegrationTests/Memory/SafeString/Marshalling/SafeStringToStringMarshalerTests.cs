using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Memory.SafeStringServices
{
    [TestFixture]
    public class SafeStringToStringMarshalerTests
    {
        [Test]
        public async Task String_AfterSettingSafeString_ReturnsStringOfSafeString()
        {
            // Arrange
            var sut = GetSut();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = new SafeString(); 
            await safeString.AppendAsync(expected);

            // Act
            var str = await sut.MarshalAsync(safeString);
            var actual = str.String;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        
        private static SafeStringToStringMarshaler GetSut() => new SafeStringToStringMarshaler();
    }
}