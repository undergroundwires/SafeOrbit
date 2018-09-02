
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

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal sealed class EntropyHasher : IEntropyHasher
    {
        public RandomNumberGenerator Rng { get; private set; }
        public IReadOnlyCollection<IHashAlgorithmWrapper> HashWrappers { get; private set; }
        public EntropyHasher(RandomNumberGenerator rng, IHashAlgorithmWrapper hashWrapper)
        {
            if (hashWrapper == null) throw new ArgumentNullException(nameof(hashWrapper));
            this.Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.HashWrappers = new []{ hashWrapper};
        }
        public EntropyHasher(RandomNumberGenerator rng, IReadOnlyCollection<IHashAlgorithmWrapper> hashWrappers)
        {
            this.Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.HashWrappers = hashWrappers ?? throw new ArgumentNullException(nameof(hashWrappers));
        }
        public void Dispose()
        {
            lock (this)      // Just in case two threads try to dispose me at the same time?  Whatev.  ;-)
            {
                if (Rng != null)
                {
                    try
                    {
                        Rng.Dispose();
                    }
                    catch { }
                    Rng = null;
                }
                if (HashWrappers == null)
                    return;
                try
                {
                    foreach (var hashWrapper in HashWrappers)
                    {
                        try
                        {
                            hashWrapper.Dispose();
                        }
                        catch { }
                    }
                }
                catch { }
                HashWrappers = null;
            }
        }
        ~EntropyHasher() => Dispose();
    }
}