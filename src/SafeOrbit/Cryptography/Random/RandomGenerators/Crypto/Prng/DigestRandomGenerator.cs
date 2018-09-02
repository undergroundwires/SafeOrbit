namespace SafeOrbit.Cryptography.Random.RandomGenerators.Crypto.Prng
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
        private const long CycleCount = 10;

        private long _stateCounter;
        private long _seedCounter;
        private readonly IDigest digest;
        private readonly byte[] state;
        private readonly byte[] seed;

        public DigestRandomGenerator(
            IDigest digest)
        {
            this.digest = digest;

            this.seed = new byte[digest.GetDigestSize()];
            this._seedCounter = 1;

            this.state = new byte[digest.GetDigestSize()];
            this._stateCounter = 1;
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
                var stateOff = 0;

                GenerateState();

                var end = start + len;
                for (var i = start; i < end; ++i)
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
            DigestAddCounter(_seedCounter++);
            DigestDoFinal(seed);
        }

        private void GenerateState()
        {
            DigestAddCounter(_stateCounter++);
            DigestUpdate(state);
            DigestUpdate(seed);
            DigestDoFinal(state);

            if ((_stateCounter % CycleCount) == 0)
            {
                CycleSeed();
            }
        }

        private void DigestAddCounter(long seedVal)
        {
            var seed = (ulong)seedVal;
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