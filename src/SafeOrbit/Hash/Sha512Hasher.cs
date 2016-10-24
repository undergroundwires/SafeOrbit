using System.Security.Cryptography;

namespace SafeOrbit.Hash
{
    public class Sha512Hasher : ISafeHasher
    {
        public byte[] Compute(byte[] input)
        {
            var hasher = new SHA512Managed();
            var result = hasher.ComputeHash(input);
            return result;
        }
    }
}