using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Random.TinHat.Crypto;
using SafeOrbit.Random.TinHat.Crypto.Digests;
using SafeOrbit.Random.TinHat.Crypto.Prng;

namespace SafeOrbit.Random.Tinhat
{
    /// <summary>
    /// TinHatURandom returns cryptographically strong random data.  It uses a crypto prng to generate more bytes than
    /// actually available in hardware entropy, so it's about 1,000 times faster than TinHatRandom.  For general purposes, 
    /// TinHatURandom is recommended because of its performance characteristics, but for extremely strong keys and other
    /// things that don't require a large number of bytes quickly, TinHatRandom is recommended instead.
    /// </summary>
    /// <remarks>
    /// TinHatURandom returns cryptographically strong random data.  It uses a crypto prng to generate more bytes than
    /// actually available in hardware entropy, so it's about 1,000 times faster than TinHatRandom.  For general purposes, 
    /// TinHatURandom is recommended because of its performance characteristics, but for extremely strong keys and other
    /// things that don't require a large number of bytes quickly, TinHatRandom is recommended instead.
    /// </remarks>
    /// <example><code>
    /// using tinhat;
    /// 
    /// static void Main(string[] args)
    /// {
    ///     StartEarly.StartFillingEntropyPools();  // Start gathering entropy as early as possible
    /// 
    ///     var randomBytes = new byte[32];
    ///
    ///     // Performance is highly variable.  On my system, it generated 2.00MB(minimum)/3.04MB(avg)/3.91MB(max) per second
    ///     // default TinHatURandom() constructor uses the TinHatRandom() default constructor, which uses:
    ///     //     SystemRNGCryptoServiceProvider/SHA256, 
    ///     //     ThreadedSeedGeneratorRNG/SHA256/RipeMD256Digest,
    ///     //     (if available) EntropyFileRNG/SHA256
    ///     TinHatURandom.StaticInstance.GetBytes(randomBytes);
    /// }
    /// </code></example>
    public sealed class FastRandomGenerator : RandomNumberGenerator
    {
        public static FastRandomGenerator StaticInstance = StaticInstanceLazy.Value;
        private static readonly Lazy<FastRandomGenerator> StaticInstanceLazy = new Lazy<FastRandomGenerator>(
            () => new FastRandomGenerator(SafeRandomGenerator.StaticInstance));
        // Interlocked cannot handle bools.  So using int as if it were bool.
        private const int TrueInt = 1;
        private const int FalseInt = 0;
        private int _isDisposed = FalseInt;
        private readonly DigestRandomGenerator _myPrng;
        private readonly SafeRandomGenerator _safeRandomGenerator;
        private readonly bool _safeRandomGeneratorIsMineExclusively;
        private readonly object _stateCounterLockObj = new object();
        private const int ReseedLocked = 1;
        private const int ReseedUnlocked = 0;
        private int _reseedLockInt = ReseedUnlocked;

        private const int MaxBytesPerSeedSoft = 64*1024; // See "BouncyCastle DigestRandomGenerator Analysis" comment
        private const int MaxStateCounterHard = 1024*1024; // See "BouncyCastle DigestRandomGenerator Analysis" comment

        private readonly int _digestSize;
        private int _stateCounter = MaxStateCounterHard; // Guarantee to seed immediately on first call to GetBytes

        /// <summary>
        /// Number of TinHatRandom bytes to use when reseeding prng
        /// </summary>
        public int SeedSize;

        /* BouncyCastle DigestRandomGenerator Analysis
         * BouncyCastle DigestRandomGenerator maintains two separate but related internal states, represented by the following:
         *     byte[] seed
         *     long   seedCounter
         *     byte[] state
         *     long   stateCounter
         * The size of seed and state are both equal to the size of the digest.  I am going to refer to the digest size, in bits,
         * as "M".  The counters are obviously 64 bits each.
         * 
         * In order to generate repeated output, there would need to be a collision of stateCounter, state, and seed.  We expect a seed
         * collision every 2^(M/2) times that we cycle seed.  We expect a state collision every 2^(M/2) times that we GenerateState,
         * and stateCounter will repeat itself every 2^64 times that we call GenerateState.  This means we can never have a repeated
         * stateCounter&state&seed in less than 2^64 calls to GenerateState, and very likely, it would be much much larger than that.
         * 
         * GenerateState is called at least once for every call to NextBytes, and it's called more times, if the number of bytes reqested
         * >= digest size in bytes.  We can easily measure the number of calls to GenerateState, by counting 1+(bytes.Length/digest.Size),
         * and we want to ensure this number is always below 2^64, which is UInt64.MaxValue
         * 
         * bytes.Length is an Int32.  We can easily guarantee we'll never repeat an internal state, if we use a UInt64 to tally the
         * number of calls to GenerateState, and require new seed material before UInt64.MaxValue - Int32.MaxValue.  This is a huge number.
         * 
         * To put this in perspective, supposing a 128 bit digest, and supposing the user on average requests 8 bytes per call to NextBytes.
         * Then there is guaranteed to be no repeat state before 147 quintillion bytes (147 billion billion).  So let's just tone this 
         * down a bit, and choose thresholds that are way more conservative.
         * 
         * Completely unrelated to analysis of DigestRandomGenerator, some other prng's (fortuna) recommend new seed material in 2^20
         * iterations, due to limitations they have, which we don't have.  So let's just ensure we end up choosing thresholds that are down
         * on-par with that level, even though completely unnecessary for us, it will feel conservative and safe.
         * 
         * Let's use a plain old int to tally the number of calls to GenerateState.  We need to ensure we never overflow this counter, so
         * let's assume all digests are at least 4 bytes, and let's require new seed material every int.MaxValue/2.  This is basically 
         * 1 billion calls to NextBytes, so a few GB of random data or so.  Extremely safe and conservative.
         * 
         * But let's squish it down even more than that.  TinHatURandom performs approx 1,000 times faster than TinHatRandom.  So to 
         * maximize the sweet spot between strong security and good performance, let's only stretch the entropy 1,000,000 times at hard 
         * maximum, and 64,000 times softly suggested.  Typically, for example with Sha256, this means we'll generate up to 2MB before 
         * requesting reseed, and up to 32MB before requiring reseed.
         * 
         * Now we're super duper conservative, being zillions of times more conservative than necessary, maximally conservative to the point 
         * where we do not take an appreciable performance degradation.
         */

        public FastRandomGenerator()
        {
            this._safeRandomGenerator = new SafeRandomGenerator();
            this._safeRandomGeneratorIsMineExclusively = true;
            IDigest digest = new Sha512Digest();
            this._myPrng = new DigestRandomGenerator(digest);
            this._digestSize = digest.GetDigestSize();
            this.SeedSize = this._digestSize;
            Reseed();
        }

        public FastRandomGenerator(IDigest digest)
        {
            this._safeRandomGenerator = new SafeRandomGenerator();
            this._safeRandomGeneratorIsMineExclusively = true;
            this._myPrng = new DigestRandomGenerator(digest);
            this._digestSize = digest.GetDigestSize();
            this.SeedSize = this._digestSize;
            Reseed();
        }

        public FastRandomGenerator(List<IEntropyHasher> entropyHashers, IDigest digest)
        {
            this._safeRandomGenerator = new SafeRandomGenerator(entropyHashers);
            this._safeRandomGeneratorIsMineExclusively = true;
            this._myPrng = new DigestRandomGenerator(digest);
            this._digestSize = digest.GetDigestSize();
            this.SeedSize = this._digestSize;
            Reseed();
        }

        public FastRandomGenerator(SafeRandomGenerator myTinHatRandom)
        {
            this._safeRandomGenerator = myTinHatRandom;
            this._safeRandomGeneratorIsMineExclusively = false;
            IDigest digest = new Sha512Digest();
            this._myPrng = new DigestRandomGenerator(digest);
            this._digestSize = digest.GetDigestSize();
            this.SeedSize = this._digestSize;
            Reseed();
        }

        public FastRandomGenerator(SafeRandomGenerator myTinHatRandom, IDigest digest)
        {
            this._safeRandomGenerator = myTinHatRandom;
            this._safeRandomGeneratorIsMineExclusively = false;
            this._myPrng = new DigestRandomGenerator(digest);
            this._digestSize = digest.GetDigestSize();
            this.SeedSize = this._digestSize;
            Reseed();
        }

        public override void GetBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) return;
            lock (_stateCounterLockObj)
            {
                var newStateCounter = this._stateCounter + 1 + (data.Length/this._digestSize);
                if (newStateCounter > MaxStateCounterHard)
                {
                    Reseed(); // Guarantees to reset stateCounter = 0
                }
                else if (newStateCounter > MaxBytesPerSeedSoft)
                {
                    if (Interlocked.Exchange(ref _reseedLockInt, ReseedLocked) == ReseedUnlocked)
                        // If more than one thread race here, let the first one through, and others exit
                    {
                        // System.Console.Error.Write(".");
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ReseedCallback));
                    }
                }
                // Repeat the addition, instead of using newStateCounter, because the above Reseed() might have changed stateCounter
                this._stateCounter += 1 + (data.Length/this._digestSize);
                _myPrng.NextBytes(data);
                    // Internally, DigestRandomGenerator locks all operations, so reseeding cannot occur in the middle of NextBytes()
            }
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            // Apparently, the reason for GetNonZeroBytes to exist, is sometimes people generate null-terminated salt strings.
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) return;
            var pos = 0;
            while (true)
            {
                var tempData = new byte[(int) (1.05*(data.Length - pos))];
                    // Request 5% more data than needed, to reduce the probability of repeating loop
                GetBytes(tempData);
                for (var i = 0; i < tempData.Length; i++)
                {
                    if (tempData[i] != 0)
                    {
                        data[pos] = tempData[i];
                        pos++;
                        if (pos == data.Length)
                        {
                            Array.Clear(tempData, 0, tempData.Length);
                            return;
                        }
                    }
                }
            }
        }

        private void ReseedCallback(object state)
        {
            Reseed();
        }

        private void Reseed()
        {
            // If we were already disposed, it's nice to skip any attempt at GetBytes, etc.
            if (_isDisposed == TrueInt)
            {
                return;
            }
            // Even though we just checked to see if we're disposed, somebody could call Dispose() while I'm in the middle
            // of the following code block.
            try
            {
                var newSeed = new byte[SeedSize];
                _safeRandomGenerator.GetBytes(newSeed);
                lock (_stateCounterLockObj)
                {
                    _myPrng.AddSeedMaterial(newSeed);
                    this._stateCounter = 0;
                    _reseedLockInt = ReseedUnlocked;
                }
            }
            catch
            {
                // If we threw any kind of exception after we were disposed, then just swallow it and go away quietly
                if (_isDisposed == FalseInt)
                {
                    throw;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _isDisposed, TrueInt) == TrueInt)
            {
                return;
            }
            if (_safeRandomGeneratorIsMineExclusively)
            {
                // If myTinHatRandom is a private instance that I created, I no longer need it, and we can get rid of it.
                // If myTinHatRandom was given to me by user in constructor (for example, if I'm the TinHatURandom.StaticInstance)
                // then I don't want to dispose of it, as somebody else might be referencing it.
                _safeRandomGenerator.Dispose();
            }
            base.Dispose(disposing);
        }

        ~FastRandomGenerator()
        {
            Dispose(false);
        }

    }
}