
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeObjectFactory" />
    public class SafeObjectFactoryFaker : StubProviderBase<ISafeObjectFactory>
    {
        public override ISafeObjectFactory Provide() => new FakeSafeObjectFactory();

        private class FakeSafeObjectFactory : ISafeObjectFactory
        {
            public ISafeObject<T> Get<T>(IInitialSafeObjectSettings settings) where T : class, new()
            {
                var mock = new Mock<ISafeObject<T>>();
                var obj = settings.InitialValue == null ? new T() : (T)settings.InitialValue;
                mock.Setup(m => m.AlertChannel).Returns(settings.AlertChannel);
                mock.Setup(m => m.CurrentProtectionMode).Returns(settings.ProtectionMode);
                mock.Setup(m => m.ApplyChanges(It.IsAny<Action<T>>())).Callback(
                    (Action<T> action) => action.Invoke(obj));
                mock.SetupGet(m => m.Object).Returns(obj);
                return mock.Object;
            }
        }
    }
}