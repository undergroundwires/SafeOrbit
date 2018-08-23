﻿
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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

namespace SafeOrbit.Cryptography.Encryption
{
    public enum BlowfishCipherMode
    {
        /// <summary>
        ///     <p>
        ///         This is the fastest mode. It's not safe at all and it should only be used on very small
        ///         or random generated data.
        ///     </p>
        /// </summary>
        /// <remarks>
        ///     The Electronic Codebook (<b>ECB</b>) mode encrypts each block individually. Any blocks of plain text that are
        ///     identical and in the same message, or that are in a different message encrypted with the same key, will be
        ///     transformed into identical cipher text blocks. <b>Important:</b>  This mode is not recommended because it opens the
        ///     door for multiple security exploits. If the plain text to be encrypted contains substantial repetition, it is
        ///     feasible for the cipher text to be broken one block at a time. It is also possible to use block analysis to
        ///     determine the encryption key. Also, an active adversary can substitute and exchange individual blocks without
        ///     detection, which allows blocks to be saved and inserted into the stream at other points without detection.
        /// </remarks>
        Ecb = 0,

        /// <summary>
        ///     The Cipher Block Chaining (<b>CBC</b>) mode introduces feedback. Before each plain text block is encrypted, it is
        ///     combined with the cipher text of the previous block by a bitwise exclusive OR operation. This ensures that even if
        ///     the plain text contains many identical blocks, they will each encrypt to a different cipher text block. The
        ///     initialization vector is combined with the first plain text block by a bitwise exclusive OR operation before the
        ///     block is encrypted. If a single bit of the cipher text block is mangled, the corresponding plain text block will
        ///     also be mangled. In addition, a bit in the subsequent block, in the same position as the original mangled bit, will
        ///     be mangled.
        /// </summary>
        Cbc = 1
    }
}