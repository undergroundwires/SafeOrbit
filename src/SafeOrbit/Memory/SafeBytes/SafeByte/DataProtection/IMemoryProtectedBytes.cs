﻿using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public interface IMemoryProtectedBytes : IDisposable, IDeepCloneable<IMemoryProtectedBytes>
    {
        void Initialize(byte[] plainBytes);
        int BlockSizeInBytes { get; }
        bool IsInitialized { get; }
        IDecryptionSession RevealDecryptedBytes();
    }
}