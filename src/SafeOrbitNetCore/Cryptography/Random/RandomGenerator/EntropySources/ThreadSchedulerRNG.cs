
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Memory;

namespace SafeOrbit.Random.Tinhat
{
    /// <summary>
    /// In a multitasking OS, each individual thread never knows when it's going to be granted execution time,
    /// as many processes and threads compete for CPU cycles.  The granularity of time to wake up from sleep is
    /// something like +/- a few ms, while the granularity of DateTime.Now is Ticks, 10million per second.  Although
    /// the OS scheduler is surely deterministic, there should be a fair amount of entropy in the least significant
    /// bits of DateTime.Now.Ticks upon thread waking.  But since the OS scheduler is surely deterministic, it is
    /// not recommended to use ThreadSchedulerRNG as your only entropy source.  It is recommended to use this
    /// class ONLY in addition to other entropy sources.
    /// </summary>
    public sealed class ThreadSchedulerRng : RandomNumberGenerator
    {
        /// <summary>
        /// By putting the core into its own class, it makes it easy for us to create a single instance of it, referenced 
        /// by a static member of ThreadSchedulerRNG, without any difficulty of finalizing & disposing etc.
        /// </summary>
        private class ThreadSchedulerRngCore
        {
            private const int MaxPoolSize = 4096;
            private object _fifoStreamLock = new object();
            private SafeMemoryStream _safeStream = new SafeMemoryStream(zeroize: true);
            private Thread _mainThread;
            private AutoResetEvent _mainThreadLoopAre = new AutoResetEvent(false);
            private AutoResetEvent _bytesAvailableAre = new AutoResetEvent(false);

            // Interlocked cannot handle bools.  So using int as if it were bool.
            private const int TrueInt = 1;
            private const int FalseInt = 0;
            private int _disposed = FalseInt;

            private const int ChunkSize = 16;
            private byte[] _chunk;
            private int _chunkByteIndex = 0;
            private int _chunkBitIndex = 0;

            public ThreadSchedulerRngCore()
            {
                _chunk = new byte[ChunkSize];
                _mainThread = new Thread(new ThreadStart(MainThreadLoop));
                _mainThread.IsBackground = true; // Don't prevent application from dying if it wants to.
                _mainThread.Start();
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                int pos = offset;
                try
                {
                    lock (_fifoStreamLock)
                    {
                        while (pos < offset + count)
                        {
                            long readCount = _safeStream.Length; // All the available bytes
                            if (pos + readCount >= offset + count)
                            {
                                readCount = offset + count - pos; // Don't try to read more than we need
                            }
                            int bytesRead = -1;
                            while (readCount > 0 && bytesRead != 0)
                            {
                                bytesRead = _safeStream.Read(buffer, pos, (int) readCount);
                                _mainThreadLoopAre.Set();
                                readCount -= bytesRead;
                                pos += bytesRead;
                            }
                            if (pos < offset + count)
                            {
                                _bytesAvailableAre.WaitOne();
                            }
                        }
                        return count;
                    }
                }
                catch
                {
                    if (_disposed == TrueInt)
                    {
                        throw new System.IO.IOException("Read() interrupted by Dispose()");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            private void MainThreadLoop()
            {
                try
                {
                    while (this._disposed == FalseInt)
                    {
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

                            long ticks = DateTime.Now.Ticks;
                            byte newBit = 0;
                            for (int i = 0; i < 64; i++) // Mix all 64 bits together to produce a single output bit
                            {
                                newBit ^= (byte) (ticks%2);
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
                }
                catch
                {
                    if (_disposed == FalseInt) // If we caught an exception after being disposed, just swallow it.
                    {
                        throw;
                    }
                }
            }

            private void GotBit(byte bitByte)
            {
                if (bitByte > 1)
                {
                    throw new ArgumentException("bitByte must be equal to 0 or 1");
                }
                _chunk[_chunkByteIndex] <<= 1; // << operator discards msb's and zero-fills lsb's, never causes overflow
                if (bitByte == 1)
                {
                    _chunk[_chunkByteIndex]++; // By incrementing, we are setting the lsb to 1.
                }
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

            private void Dispose(bool disposing)
            {
                if (Interlocked.Exchange(ref _disposed, TrueInt) == TrueInt)
                {
                    return;
                }
                _mainThreadLoopAre.Set();
                _mainThreadLoopAre.Dispose();
                _bytesAvailableAre.Set();
                _bytesAvailableAre.Dispose();
                _safeStream.Dispose();
            }

            ~ThreadSchedulerRngCore()
            {
                Dispose(false);
            }
        }

        private static ThreadSchedulerRngCore _core = new ThreadSchedulerRngCore();

        public override void GetBytes(byte[] data)
        {
            if (_core.Read(data, 0, data.Length) != data.Length)
            {
                throw new CryptographicException("Failed to return requested number of bytes");
            }
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                var newBytes = new byte[data.Length - offset];
                if (_core.Read(newBytes, 0, newBytes.Length) != newBytes.Length)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        ~ThreadSchedulerRng()
        {
            Dispose(false);
        }
    }
}