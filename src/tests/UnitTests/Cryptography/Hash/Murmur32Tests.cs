using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Cryptography.Hashers
{
    /// <seealso cref="Murmur32" />
    /// <seealso cref="IFastHasher" />
    [TestFixture]
    public class Murmur32Tests : TestsFor<Murmur32>
    {
        protected override Murmur32 GetSut() => new Murmur32();

        [Test]
        public void ComputeBytesFast_VerifyTheVerificationKey_returnsTrue()
        {
            //Arrange
            uint verificationKey = 0xB0F57EE3; //https://github.com/rurban/smhasher/blob/8542a35b4d682f6c80754668cff32069a70eecc0/main.cpp
            var sut = GetSut();
            //Act
            int result;
            //http://stackoverflow.com/questions/14747343/murmurhash3-test-vectors
            using (var stream = new MemoryStream())
            {
                var key = new byte[256];

                for (var i = 0; i < 256; i++)
                {
                    key[i] = (byte)i;
                    var seed = (uint)(256 - i);
                    var computed = sut.ComputeFast(key.Take(i).ToArray(), seed);
                    stream.Write(BitConverter.GetBytes(computed), 0, 4);
                }
                ArraySegment<byte> buffer;
                if(!stream.TryGetBuffer(out buffer)) Assert.False(false, "Could not get the buffer.");
                result = sut.ComputeFast(buffer.Array, 0);
            }
            //Assert
            Assert.That(result, Is.EqualTo((int)verificationKey));
        }
        [Test]
        [TestCase(0xCC9E2D51, new byte[] { 0x22, 0x90, 0x63, 0xfa })]
        public void ComputeBytesFast_ForSameSeed_FromDifferentInstances_returnsSame(uint seed, byte[] bytes)
        {
            //Arrange
            var sut = GetSut();
            var expected = sut.ComputeFast(bytes, seed);
            var hashes = new List<int>();
            //act
            for (int i = 0; i < 20; i++)
            {
                var differentInstance = GetSut();
                var hash = differentInstance.ComputeFast(bytes, seed);
                hashes.Add(hash);
            }
            //Assert
            Assert.That(hashes, Has.All.EqualTo(expected));
        }
        [Test]
        [TestCase(0xCC9E2D51, new byte[] { 0x22, 0x90, 0x63, 0xfa })]
        public void ComputeBytesFast_ForSameSeed_FromDifferentInstancesAndThreads_returnsSame(uint seed, byte[] bytes)
        {
            //Arrange
            var totalTests = 20;
            var sut = GetSut();
            var expected = sut.ComputeFast(bytes, seed);
            var threads = new Thread[totalTests];
            var hashes = new ConcurrentBag<int>();
            //act
            for (var i = 0; i < totalTests; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var differentInstance = GetSut();
                    var hash = differentInstance.ComputeFast(bytes, seed);
                    hashes.Add(hash);
                });
                threads[i].Start();
            }
            for (var i = 0; i < totalTests; i++)
            {
                threads[i].Join();
            }
            //Assert
            Assert.That(hashes, Has.All.EqualTo(expected));
        }
        [Test]
        [TestCase(0xCC9E2D51, 0xE6546B64, new byte[] { 0x22, 0x90, 0x63, 0xfa })]
        public void ComputeBytesFast_ResultsForTheSameBytesWithDifferentSeeds_areDifferent(uint seed1, uint seed2, byte[] bytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var hash1 = sut.ComputeFast(bytes, seed1);
            var hash2 = sut.ComputeFast(bytes, seed2);
            //Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }
    }
}
