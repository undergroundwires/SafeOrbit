using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Helpers;
using SafeOrbit.Memory;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <inheritdoc />
    /// <summary>
    ///     In a multitasking OS, each individual thread never knows when it's going to be granted execution time,
    ///     as many processes and threads compete for CPU cycles.  The granularity of time to wake up from sleep is
    ///     something like +/- a few ms, while the granularity of DateTime.Now is Ticks, 10million per second.  Although
    ///     the OS scheduler is surely deterministic, there should be a fair amount of entropy in the least significant
    ///     bits of DateTime.Now.Ticks upon thread waking.  But since the OS scheduler is surely deterministic, it is
    ///     not recommended to use ThreadSchedulerRNG as your only entropy source.  It is recommended to use this
    ///     class ONLY in addition to other entropy sources.
    /// </summary>
    public sealed class ThreadSchedulerRng : RandomNumberGenerator
    {
        private static readonly ThreadSchedulerRngCore Core = new ThreadSchedulerRngCore();

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="CryptographicException">Failed to return requested number of bytes</exception>
        public override void GetBytes(byte[] data)
        {
            if (Core.Read(data, 0, data.Length) != data.Length)
                throw new CryptographicException("Failed to return requested number of bytes");
        }

#if !NETSTANDARD1_6
        public override void GetNonZeroBytes(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                var newBytes = new byte[data.Length - offset];
                if (Core.Read(newBytes, 0, newBytes.Length) != newBytes.Length)
                {
                    throw new CryptographicException("Failed to return requested number of bytes");
                }
                for (int i = 0; i < newBytes.Length; i++)
                {
                    if (newBytes[i] != 0)
                    {
                        data[offset] = newBytes[i];
                        offset++;
                    }
                }
            }
        }
#endif

        ~ThreadSchedulerRng()
        {
            Dispose(false);
        }

        /// <summary>
        ///     By putting the core into its own class, it makes it easy for us to create a single instance of it, referenced
        ///     by a static member of ThreadSchedulerRNG, without any difficulty of finalizing or disposing etc.
        /// </summary>
        private class ThreadSchedulerRngCore
        {
            private const int MaxPoolSize = 4096;

            private const int ChunkSize = 16;
            private readonly object _fifoStreamLock = new object();
            private readonly SafeMemoryStream _safeStream = new SafeMemoryStream();
            private readonly AutoResetEvent _bytesAvailableAre = new AutoResetEvent(false);
            private readonly byte[] _chunk;
            private int _chunkBitIndex;
            private int _chunkByteIndex;
            private int _disposed = IntCondition.False;
            private readonly Thread _mainThread;
            private readonly AutoResetEvent _mainThreadLoopAre = new AutoResetEvent(false);

            public ThreadSchedulerRngCore()
            {
                _chunk = new byte[ChunkSize];
                _mainThread = new Thread(MainThreadLoop)
                {
                    IsBackground = true // Don't prevent application from dying if it wants to.
                };
                _mainThread.Start();
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                var pos = offset;
                try
                {
                    lock (_fifoStreamLock)
                    {
                        while (pos < offset + count)
                        {
                            var readCount = _safeStream.Length; // All the available bytes
                            if (pos + readCount >= offset + count)
                                readCount = offset + count - pos; // Don't try to read more than we need
                            var bytesRead = -1;
                            while ((readCount > 0) && (bytesRead != 0))
                            {
                                bytesRead = _safeStream.Read(buffer, pos, (int)readCount);
                                _mainThreadLoopAre.Set();
                                readCount -= bytesRead;
                                pos += bytesRead;
                            }
                            if (pos < offset + count)
                                _bytesAvailableAre.WaitOne();
                        }
                        return count;
                    }
                }
                catch
                {
                    if (_disposed == IntCondition.True)
                        throw new IOException($"{nameof(Read)}() interrupted by {nameof(Dispose)}()");
                    throw;
                }
            }

            private void MainThreadLoop()
            {
                try
                {
                    while (_disposed == IntCondition.False)
                        if (_safeStream.Length < MaxPoolSize)
                        {
                            // While running in this tight loop, consumes approx 0% cpu time.  Cannot even measure with Task Manager

                            /* With 10m ticks per second, and Thread.Sleep() precision of 1ms, it means Ticks is 10,000 times more precise than
                             * the sleep wakeup timer.  This means there could exist as much as 14 bits of entropy in every thread wakeup cycle,
                             * but realistically that's completely unrealistic.  I ran this 64*1024 times, and benchmarked each bit individually.
                             * The estimated entropy bits per bit of Ticks sample is very near 1 bit for each of the first 8 bits, and quickly
                             * deteriorates after that.
                             * 
                             * Surprisingly, the LSB #0 and LSB #1 demonstrated the *least* entropy within the first 8 bits, but it was still
                             * 0.987 bits per bit, which could be within sampling noise.  Bits 9, 10, and beyond very clearly demonstrated a
                             * degradation in terms of entropy quality.
                             * 
                             * The estimated sum total of all entropy in all 64 bits is about 14 bits of entropy per sample, which is just 
                             * coincidentally the same as the difference in precision described above.
                             * 
                             * Based on superstition, I would not attempt to extract anywhere near 14 bits per sample, not even 8 bits. But since 
                             * the first 8 bits all measured to be very close to 1 bit per bit, I am comfortable extracting at least 2 or 4 bits.
                             * 
                             * To be ultra-conservative, I'll extract only a single bit each time, and it will be a mixture of all 64 bits.  Which
                             * means, as long as *any* bit is unknown to an adversary, or the sum total of the adversary's uncertainty over all 64
                             * bits > 50%, then the adversary will have at best 50% chance of guessing the output bit, which means it is 1 bit of
                             * good solid entropy.  In other words, by mashing all ~8-14 bits of entrpoy into a single bit, the resultant bit
                             * should be a really good quality entropy bit.
                             */

                            var ticks = DateTime.Now.Ticks;
                            byte newBit = 0;
                            for (var i = 0; i < 64; i++) // Mix all 64 bits together to produce a single output bit
                            {
                                newBit ^= (byte)(ticks % 2);
                                ticks >>= 1;
                            }
                            GotBit(newBit);
                            Thread.Sleep(1);
                        }
                        else
                        {
                            _mainThreadLoopAre.WaitOne();
                        }
                }
                catch
                {
                    if (_disposed == IntCondition.False) // If we caught an exception after being disposed, just swallow it.
                        throw;
                }
            }

            private void GotBit(byte bitByte)
            {
                if (bitByte > 1)
                    throw new ArgumentOutOfRangeException(nameof(bitByte), $"{nameof(bitByte)} must be equal to 0 or 1");
                _chunk[_chunkByteIndex] <<= 1; // << operator discards msb's and zero-fills lsb's, never causes overflow
                if (bitByte == 1)
                    _chunk[_chunkByteIndex]++; // By incrementing, we are setting the lsb to 1.
                _chunkBitIndex++;
                if (_chunkBitIndex > 7)
                {
                    _chunkBitIndex = 0;
                    _chunkByteIndex++;
                    if (_chunkByteIndex >= ChunkSize)
                    {
                        _safeStream.Write(_chunk, 0, ChunkSize);
                        _bytesAvailableAre.Set();
                        _chunkByteIndex = 0;
                    }
                }
            }

            private void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, IntCondition.True) == IntCondition.True)
                    return;
                _mainThreadLoopAre.Set();
                _mainThreadLoopAre.Dispose();
                _bytesAvailableAre.Set();
                _bytesAvailableAre.Dispose();
                _safeStream.Dispose();
            }

            ~ThreadSchedulerRngCore() => Dispose();
        }
    }
}