using System;
using System.Diagnostics;

namespace SafeOrbit.Core.Reflection
{
    /// <inheritdoc />
    /// <summary>
    ///     Generates a key/id for each type
    /// </summary>
    /// <seealso cref="T:SafeOrbit.Core.Reflection.ITypeIdGenerator" />
    public class TypeIdGenerator : ITypeIdGenerator
    {
        private static readonly Lazy<TypeIdGenerator> StaticInstanceLazy = new Lazy<TypeIdGenerator>();

        /// <summary>
        ///     Gets the static instance.
        /// </summary>
        /// <value>The static instance.</value>
        public static ITypeIdGenerator StaticInstance => StaticInstanceLazy.Value;

        /// <inheritdoc />
        /// <summary>
        /// Generates a new id for the type.
        /// </summary>
        /// <typeparam name="T">The type that the id will be returned for.</typeparam>
        /// <returns>The id.</returns>
        public string Generate<T>() => Generate(typeof(T));

        /// <inheritdoc />
        /// <summary>
        /// Generates a new id for the type.
        /// </summary>
        /// <param name="type">The type that the id will be returned for.</param>
        /// <returns>The id.</returns>
        [DebuggerHidden]
        public string Generate(Type type) => type.AssemblyQualifiedName.Replace(" ", "");
    }
}