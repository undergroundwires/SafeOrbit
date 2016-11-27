
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

using SafeOrbit.Cryptography.Random.RandomGenerators;

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

            InvokeStaticConstructorFor<FastRandomGenerator>();
            InvokeStaticConstructorFor<SafeRandomGenerator>();
        }
    }
}