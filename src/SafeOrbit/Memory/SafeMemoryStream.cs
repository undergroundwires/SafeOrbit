using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Creates a stream with no backing store (ephemeral memory). The buffer will be deleted after it's read or written.
    /// </summary>
    /// <seealso cref="Stream" />
    /// <seealso cref="SafeMemoryStream.Read(byte[], int,int)" />
    public class SafeMemoryStream : Stream
    {
        private readonly AutoResetEvent _flushAutoResetEvent = new AutoResetEvent(false);
        private readonly Queue<byte[]> _queue = new Queue<byte[]>();
        private readonly AutoResetEvent _readerAutoResetEvent = new AutoResetEvent(false);
        private readonly object _readLock = new object();
        private bool _closed;
        private byte[] _currentBlock;
        private int _currentBlockPosition;
        private long _length;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => !_closed;
        public override long Length => _length;

        /// <exception cref="NotSupportedException" accessor="set">
        ///     <see cref="Position" /> is not supported for
        ///     <see cref="SafeMemoryStream" />.
        /// </exception>
        /// <exception cref="NotSupportedException" accessor="get">
        ///     <see cref="Position" /> is not supported for
        ///     <see cref="SafeMemoryStream" />.
        /// </exception>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <exception cref="NotSupportedException">
        ///     <see cref="Seek" /> is not supported for <see cref="SafeMemoryStream" />
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <exception cref="NotSupportedException">This method is not supported on a <see cref="SafeMemoryStream" /></exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <exception cref="AbandonedMutexException">
        ///     The wait completed because a thread exited without releasing a mutex. This
        ///     exception is not thrown on Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The current instance is a transparent proxy for a
        ///     <see cref="WaitHandle" /> in another application domain.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed. </exception>
        public override void Flush()
        {
            while (_length > 0)
                _flushAutoResetEvent.WaitOne();
        }

        /// <summary>
        ///     Reads from the buffer and deletes the read data from the <paramref name="buffer" />.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="buffer" />'s length is out of range.</exception>
        /// <exception cref="ArgumentException">Condition.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureReadOrWriteParameters(buffer, offset, count);
            if (_closed) throw new ArgumentException(nameof(_closed));
            var newBytes = new byte[count];
            Array.Copy(buffer, offset, newBytes, 0, count);
            Array.Clear(buffer, offset, count);
            lock (_queue)
            {
                _queue.Enqueue(newBytes);
                _length += newBytes.Length;
                _readerAutoResetEvent.Set();
            }
        }

        /// <summary>
        ///     Reads from the buffer and deletes the read data from the buffer.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="buffer" />'s length is out of range.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            EnsureReadOrWriteParameters(buffer, offset, count);
            if (_closed && _length == 0)
                return 0;
            var finalPosition = offset + count;
            var position = offset;
            var bytesRead = 0;
            lock (_readLock)
            {
                var internalLength = _length;
                while (position < finalPosition && (false == _closed || internalLength > 0))
                {
                    if (_currentBlock == null)
                        lock (_queue)
                        {
                            if (_queue.Count > 0)
                                _currentBlock = _queue.Dequeue();
                        }

                    if (_currentBlock == null)
                    {
                        if (!_closed)
                            _readerAutoResetEvent.WaitOne();
                        continue;
                    }

                    var bytesRequested = finalPosition - position;
                    var bytesAvail = _currentBlock.Length - _currentBlockPosition;
                    var bytesToRead = bytesRequested <= bytesAvail ? bytesRequested : bytesAvail;
                    Array.Copy(_currentBlock, _currentBlockPosition, buffer, position, bytesToRead);
                    Array.Clear(_currentBlock, _currentBlockPosition, bytesToRead);
                    internalLength -= bytesToRead;
                    position += bytesToRead;
                    _currentBlockPosition += bytesToRead;
                    bytesRead += bytesToRead;
                    if (_currentBlockPosition != _currentBlock.Length) continue;
                    _currentBlock = null;
                    _currentBlockPosition = 0;
                }
            }

            _flushAutoResetEvent.Set();
            _length -= bytesRead;
            return bytesRead;
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="SafeMemoryStream" /> and optionally releases the managed
        ///     resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            _closed = true;
            _flushAutoResetEvent.Set();
            _flushAutoResetEvent.Dispose();
            _readerAutoResetEvent.Set();
            _readerAutoResetEvent.Dispose();
            base.Dispose(disposing);
        }

        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="buffer" />'s length is out of range.</exception>
        public void EnsureReadOrWriteParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            var finalPosition = offset + count;
            if (buffer.Length < finalPosition || offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException(nameof(buffer.Length));
        }
    }
}