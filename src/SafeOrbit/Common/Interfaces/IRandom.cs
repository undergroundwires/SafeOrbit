namespace SafeOrbit.Interfaces
{
    public interface IRandom
    {
        byte[] GetBytes(int length);
        int GetInt();
        int GetInt(int min, int max);
        bool GetBool();
    }
}