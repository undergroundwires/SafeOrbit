using SafeOrbit.Cryptography.Encryption.Padding.Padders;

namespace SafeOrbit.Cryptography.Encryption.Padding.Factory
{
    public interface IPadderFactory
    {
        IPadder GetPadder(PaddingMode paddingMode);
    }
}
