
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

using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeContainerServices.Instance;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IInstanceProviderFactory" />
    internal class InstanceProviderFactoryFaker : StubProviderBase<IInstanceProviderFactory>
    {
        public override IInstanceProviderFactory Provide()
        {
            var mock = new Mock<IInstanceProviderFactory>();
            mock.Setup(
                    m =>
                        m.Get<InstanceTestClass>(
                            It.IsAny<LifeTime>(),
                            It.IsAny<InstanceProtectionMode>(),
                            It.IsAny<InjectionAlertChannel>()))
                .Returns<LifeTime, InstanceProtectionMode, InjectionAlertChannel>
                ((lifeTime, protectionMode, alertChannel) =>
                {
                    var mockProvider = new Mock<IInstanceProvider>();
                    mockProvider.Setup(p => p.LifeTime).Returns(lifeTime);
                    mockProvider.Setup(p => p.CurrentProtectionMode).Returns(protectionMode);
                    mockProvider.Setup(p => p.AlertChannel).Returns(alertChannel);
                    mockProvider.Setup(p => p.Provide()).Returns(new InstanceTestClass());
                    return mockProvider.Object;
                });
            return mock.Object;
        }
    }
}