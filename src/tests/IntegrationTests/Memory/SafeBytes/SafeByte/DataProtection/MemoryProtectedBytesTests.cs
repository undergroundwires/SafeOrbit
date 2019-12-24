using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SafeOrbit.Tests;
using Shared.Extensions;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <seealso cref="MemoryProtectedBytes" />
    /// <seealso cref="IMemoryProtectedBytes" />
    [TestFixture]
    public class MemoryProtectedBytesTests : TestsFor<IMemoryProtectedBytes>
    {
        [Test]
        public void RevealDecryptedBytes_FromMultipleThreads_DoesNotCorruptData()
        {
            // arrange sut
            var sut = GetSut();
            var expected = GetTestBytes(sut.BlockSizeInBytes);
            sut.Initialize(expected.CopyToNewArray());
            // arrange threads
            const int totalThreads = 6;
            const int totalCountPerThread = 100;
            var collection = new ConcurrentBag<byte[]>();
            var threads = new Thread[totalThreads];
            var random = new Random();
            for (var i = 0; i < totalThreads; i++)
            {
                var threadId = i;
                threads[i] = new Thread(() =>
                {
                    // act
                    var count = 0;
                    while (count < totalCountPerThread)
                    {
                        Console.WriteLine($"Thread {threadId} is running for {count} time.");
                        using (var firstSession = sut.RevealDecryptedBytes())
                        {
                            var actual = firstSession.PlainBytes.CopyToNewArray();
                            collection.Add(actual);
                            var sleepMs = random.Next(0, 100);
                            Console.WriteLine($"Thread {threadId} is sleeping for {sleepMs} ms.");
                            Thread.Sleep(sleepMs);
                        }
                        count++;
                    }
                    Console.WriteLine($"Thread {threadId} is completed");
                });
            }
            // run
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            // assert
            Console.WriteLine($"Total revealed results in the bag: {collection.Count}");
            Assert.True(collection.All(bytes => expected.SequenceEqual(bytes)));
        }

        private static byte[] GetTestBytes(int length)
        {
            var buffer = new byte[length];
            for (var i = 0; i < length; i++)
                buffer[i] = (byte)i;
            return buffer;
        }

        protected override IMemoryProtectedBytes GetSut() => new MemoryProtectedBytes();
    }
}
