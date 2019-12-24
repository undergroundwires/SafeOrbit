using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace UnitTests.Memory.SafeBytes.SafeByte.DataProtection
{
    /// <seealso cref="DecryptionSession" />
    /// <seealso cref="IDecryptionSession" />
    [TestFixture]
    public class DecryptionSessionTests
    {
        private static IDecryptionSession GetSut(byte[] encryptedBytes)
        {
            return new DecryptionSession(
                Stubs.Get<IByteArrayProtector>(),
                ref encryptedBytes
                );
        }
    }
}
