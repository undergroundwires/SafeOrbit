using System;
using SafeOrbit.Cryptography.Encryption;

namespace CoreIntegrationConsoleApp.Tests
{
    public class BlowfishEncryptorTest : ITest
    {
        public void RunTest()
        {
            Console.WriteLine($"Testing {nameof(BlowfishEncryptor)}");
            var encryptor = new BlowfishEncryptor();
            var raw = new byte[] { 77 };
            var key = new byte[encryptor.MinKeySize];
            var encrypted = encryptor.Encrypt(raw, key);
            var decrypted = encryptor.Decrypt(encrypted, key);
            Console.WriteLine($"Raw : {raw.Dump()}");
            Console.WriteLine($"Key : {key.Dump()}");
            Console.WriteLine($"Encrypted : {encrypted.Dump()}");
            Console.WriteLine($"Decrypted : {decrypted.Dump()}");
        }
    }
}