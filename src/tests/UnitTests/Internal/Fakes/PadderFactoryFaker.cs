using Moq;
using SafeOrbit.Cryptography.Encryption.Padding;
using SafeOrbit.Cryptography.Encryption.Padding.Factory;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IPadderFactory" />
    internal class PadderFactoryFaker : StubProviderBase<IPadderFactory>
    {
        public override IPadderFactory Provide()
        {
            var fake = new Mock<IPadderFactory>();
            fake.Setup(f => f.GetPadder(PaddingMode.PKCS7))
                .Returns(Stubs.Get<IPkcs7Padder>());
            return fake.Object;
        }
    }
}