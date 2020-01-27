using System;

namespace SafeOrbit.Memory.InjectionServices.Reflection
{
    /// <summary>
    ///     Abstract a service that can generate unique identifiers for a type.
    /// </summary>
    public interface ITypeIdGenerator
    {
        /// <summary>
        ///     Generates a new id for the type.
        /// </summary>
        /// <typeparam name="T">The type that the id will be returned for.</typeparam>
        /// <returns>The id.</returns>
        string Generate<T>();

        /// <summary>
        ///     Generates a new id for the type.
        /// </summary>
        /// <param name="type">The type that the id will be returned for.</param>
        /// <returns>The id.</returns>
        string Generate(Type type);
    }
}