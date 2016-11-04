
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

namespace SafeOrbit.Cryptography.Random.Common.Crypto.Prng
{
    /**
         * Random generation based on the digest with counter. Calling AddSeedMaterial will
         * always increase the entropy of the hash.
         * <p>
         * Internal access to the digest is synchronized so a single one of these can be shared.
         * </p>
         */
    internal class DigestRandomGenerator
        : IRandomGenerator
    {
        private const long CYCLE_COUNT = 10;

        private long stateCounter;
        private long seedCounter;
        private IDigest digest;
        private byte[] state;
        private byte[] seed;

        public DigestRandomGenerator(
            IDigest digest)
        {
            this.digest = digest;

            this.seed = new byte[digest.GetDigestSize()];
            this.seedCounter = 1;

            this.state = new byte[digest.GetDigestSize()];
            this.stateCounter = 1;
        }

        public void AddSeedMaterial(
            byte[] inSeed)
        {
            lock (this)
            {
                DigestUpdate(inSeed);
                DigestUpdate(seed);
                DigestDoFinal(seed);
            }
        }

        public void AddSeedMaterial(
            long rSeed)
        {
            lock (this)
            {
                DigestAddCounter(rSeed);
                DigestUpdate(seed);
                DigestDoFinal(seed);
            }
        }

        public void NextBytes(
            byte[] bytes)
        {
            NextBytes(bytes, 0, bytes.Length);
        }

        public void NextBytes(
            byte[] bytes,
            int start,
            int len)
        {
            lock (this)
            {
                int stateOff = 0;

                GenerateState();

                int end = start + len;
                for (int i = start; i < end; ++i)
                {
                    if (stateOff == state.Length)
                    {
                        GenerateState();
                        stateOff = 0;
                    }
                    bytes[i] = state[stateOff++];
                }
            }
        }

        private void CycleSeed()
        {
            DigestUpdate(seed);
            DigestAddCounter(seedCounter++);
            DigestDoFinal(seed);
        }

        private void GenerateState()
        {
            DigestAddCounter(stateCounter++);
            DigestUpdate(state);
            DigestUpdate(seed);
            DigestDoFinal(state);

            if ((stateCounter % CYCLE_COUNT) == 0)
            {
                CycleSeed();
            }
        }

        private void DigestAddCounter(long seedVal)
        {
            ulong seed = (ulong)seedVal;
            for (int i = 0; i != 8; i++)
            {
                digest.Update((byte)seed);
                seed >>= 8;
            }
        }

        private void DigestUpdate(byte[] inSeed)
        {
            digest.BlockUpdate(inSeed, 0, inSeed.Length);
        }

        private void DigestDoFinal(byte[] result)
        {
            digest.DoFinal(result, 0);
        }
    }
}