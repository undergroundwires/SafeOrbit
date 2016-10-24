using SafeOrbit.Random.Tinhat;

namespace SafeOrbit.Library.StartEarly
{
    internal class StartFillingEntropyPoolsStartEarlyTask : StartEarlyTaskBase
    {
        public override void Prepare()
        {
            /* We could instantiate each of these individually - as we formerly did - 
             * But we could also just instantiate TinHatURandom() once, which will instantiate TinHatRandom, 
             * which will instantiate everything else that it uses by default.
             * So let's do that.
             * 
            var threadRNG = new EntropySources.ThreadSchedulerRNG();
            threadRNG.Dispose();
            var threadedSeedRNG = new EntropySources.ThreadedSeedGeneratorRNG();
            threadedSeedRNG.Dispose();
            var systemRNG = new EntropySources.SystemRNGCryptoServiceProvider();
            systemRNG.Dispose();
            // Also by referencing TinHatRandom.StaticInstance once, we force it to be created
            var junkString = TinHatRandom.StaticInstance.ToString();
             */

            // Just do anything that references StaticInstance, in order to make StaticInstance run through its
            // static constructor stuff

          //  InvokeStaticConstructorFor<FastRandomGenerator>();
        }
    }
}