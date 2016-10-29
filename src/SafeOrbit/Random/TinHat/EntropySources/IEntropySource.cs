namespace SafeOrbit.Random.Tinhat
{
    public interface IEntropySource
    {
        byte[] GetBytes();

        bool CanGetNonZeroBytes { get; }
        byte[] GetNonZeroBytes();
    }
}