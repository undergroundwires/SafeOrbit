using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Random
{
    /// <summary>
    /// Helper methods for the implementations.
    /// </summary>
    public abstract class RandomBase
    {
        /// <exception cref="ArgumentOutOfRangeException"><param name="upperBound"/> must be greater than <param name="lowerBound"/></exception>
        protected void EnsureParameters(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException(nameof(upperBound), upperBound, $"{nameof(upperBound)} must be greater than {nameof(lowerBound)}");
            }
        }
    }
}
