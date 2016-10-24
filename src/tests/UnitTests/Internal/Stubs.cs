using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SafeOrbit.Interfaces;

namespace SafeOrbit.UnitTests
{
    using Moq;
    using SafeOrbit.Tests;
    public static class Stubs
    {
        private static StubFactory _factory;

        public static T Get<T>()
        {
            if(_factory == null) _factory = GetFactory();
            return _factory.Provide<T>();
        }
        public static IFactory<T> GetFactory<T>()
        {
            if (_factory == null) _factory = GetFactory();
            var mock = new Mock<IFactory<T>>();
            mock.Setup(f => f.Create()).Returns(Get<T>);
            return mock.Object;
        }
        private static StubFactory GetFactory()
        {
            var factory = new StubFactory();
            factory.RegisterAll(Assembly.GetExecutingAssembly());
            return factory;
        }
    }
}
