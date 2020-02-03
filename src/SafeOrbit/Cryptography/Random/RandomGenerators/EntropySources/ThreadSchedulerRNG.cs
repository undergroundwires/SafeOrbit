using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Common;
using SafeOrbit.Memory;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <inheritdoc />
    /// <summary>It is recommended to use this
    ///     class ONLY in addition to other entropy sources.
    /// </summary>
    /// <remarks>
    ///     In a multitasking OS, each individual thread never knows when it's going to be granted execution time,
    ///     as many processes and threads compete for CPU cycles.  The granularity of time to wake up from sleep is
    ///     something like +/- a few ms, while the granularity of <see cref="DateTime.Now"/> is <seealso cref="DateTime.Ticks"/>,
    ///     10million per second. Although the OS scheduler is surely deterministic, there should be a fair amount of entropy in
    ///     the least significant bits of <see cref="DateTime.Ticks"/> of <see cref="DateTime.Now"/> upon thread waking.
    ///     But since the OS scheduler is surely deterministic, this entropy source should not be used as the only source but
    ///     ONLY in addition to other entropy sources.
    /// </remarks>
    public sealed class ThreadSchedulerRng : RandomNumberGenerator
    {
        private readonly ThreadSchedulerRngCore _core = new ThreadSchedulerRngCore();

        /// <exception cref="CryptographicException">Failed to return requested number of bytes</exception>
        /// <exception cref="ObjectDisposedException">Throws if the instance is disposed.</exception>
        public override void GetBytes(byte[] data)
        {
            if (_core.Read(data, 0, data.Length) != data.Length)
                throw new CryptographicException("Failed to return requested number of bytes");
        }

#if !NETSTANDARD1_6
        public override void GetNonZeroBytes(byte[] data)
        {
            var offset = 0;
            while (offset < data.Length)
            {
                var newBytes = new byte[data.Length - offset];
                if (_core.Read(newBytes, 0, newBytes.Length) != newBytes.Length)
                    throw new CryptographicException("Failed to return requested number of bytes");
                foreach (var newByte in newBytes)
                {
                    if (newByte == 0)
                        continue;
                    data[offset] = newByte;
                    offset++;
                }
            }
        }
#endif
        protected override void Dispose(bool disposing)
        {
           if(disposing)
               _core?.Dispose();
        }
        /// <summary>
        ///     By putting the core into its own class, it makes it easy for us to create a single instance of it, referenced
        ///     by a static member of ThreadSchedulerRNG, without any difficulty of finalizing or disposing etc.
        /// </summary>
        private class ThreadSchedulerRngCore : DisposableBase
        {
            private const int MaxPoolSize = 4096;

            private const int ChunkSize = 16;
            private readonly AutoResetEvent _bytesAvailableAre = new AutoResetEvent(false);
            private readonly byte[] _chunk;
            private readonly object _fifoStreamLock = new object();
            private readonly AutoResetEvent _mainThreadLoopAre = new AutoResetEvent(false);
            private readonly SafeMemoryStream _safeStream = new SafeMemoryStream();
            private int _chunkBitIndex;
            private int _chunkByteIndex;

            public ThreadSchedulerRngCore()
            {
                _chunk = new byte[ChunkSize];
                var mainThread = new Thread(MainThreadLoop)
                {
                    IsBackground = true // Don't prevent application from dying if it wants to.
                };
                mainThread.Start();
            }

            /// <see cref="DisposableBase.ThrowIfDisposed"/>
            public int Read(byte[] buffer, int offset, int count)
            {
                this.ThrowIfDisposed();
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
                            while (readCount > 0 && bytesRead != 0)
                            {
                                bytesRead = _safeStream.Read(buffer, pos, (int) readCount);
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
                catch(Exception e)
                {
                    if (IsDisposed)
                    {
                        throw new IOException($"{nameof(Read)}() interrupted by {nameof(Dispose)}()", e);
                    }
                    throw;
                }
            }

            /// <remarks>
            ///     While running in this tight loop, consumes approx 0% cpu time.  Cannot even measure with Task Manager
            /// </remarks>
            private void MainThreadLoop()
            {
                while (!IsDisposed)
                    if (_safeStream.Length < MaxPoolSize)
                    {
                        var ticks = DateTime.Now.Ticks;
                        byte newBit = 0;
                        for (var i = 0; i < 64; i++) // Mix all 64 bits together to produce a single output bit
                        {
                            newBit ^= (byte) (ticks % 2);
                            ticks >>= 1;
                        }
                        SaveBit(newBit);
                        Thread.Sleep(1);
                        /*
                         * With 10m ticks per second, and Thread.Sleep() precision of 1ms, it means Ticks is 10,000 times more precise than
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
                         * good solid entropy.  In other words, by mashing all ~8-14 bits of entropy into a single bit, the resultant bit
                         * should be a really good quality entropy bit.
                         */
                    }
                    else
                    {
                        _mainThreadLoopAre.WaitOne();
                    }
            }

            private void SaveBit(byte bitByte)
            {
                if (bitByte > 1)
                    throw new ArgumentOutOfRangeException(nameof(bitByte),
                        $"{nameof(bitByte)} must be equal to 0 or 1");
                _chunk[_chunkByteIndex] <<= 1; // << operator discards msb's and zero-fills lsb's, never causes overflow
                if (bitByte == 1)
                    _chunk[_chunkByteIndex]++; // By incrementing, we are setting the lsb to 1.
                _chunkBitIndex++;
                if (_chunkBitIndex <= 7)
                    return;
                _chunkBitIndex = 0;
                _chunkByteIndex++;
                if (_chunkByteIndex < ChunkSize)
                    return;
                _safeStream.Write(_chunk, 0, ChunkSize);
                _bytesAvailableAre.Set();
                _chunkByteIndex = 0;
            }

            protected override void DisposeManagedResources()
            {
                _mainThreadLoopAre?.Set();
                _mainThreadLoopAre?.Dispose();
                _bytesAvailableAre?.Set();
                _bytesAvailableAre?.Dispose();
                _safeStream?.Dispose();
            }
        }
    }
}