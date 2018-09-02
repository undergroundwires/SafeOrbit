using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IInjectionDetector" />
    public class InjectionDetectorFaker : StubProviderBase<IInjectionDetector>
    {
        public override IInjectionDetector Provide()
        {
            return Mock.Of<IInjectionDetector>();
        }
    }
}