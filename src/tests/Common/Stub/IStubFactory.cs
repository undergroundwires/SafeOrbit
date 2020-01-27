using System;
using System.Reflection;

namespace SafeOrbit.Tests
{
    public interface IStubFactory
    {
        void RegisterAll(Assembly assembly);
        TStub Provide<TStub>();
        void Register(Type type, IStubProvider stubProvider);
        void Register<T>(IStubProvider<T> stubProvider);
    }
}