using System;
using NUnit.Framework;
using SafeOrbit.Memory;
#if !NETCOREAPP1_1
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Exceptions
{
    /// <seealso cref="MemoryInjectionException" />
    [TestFixture]
    public class MemoryInjectionExceptionTests : SerializableExceptionTestsBase<MemoryInjectionException>
    {
        protected override MemoryInjectionException GetSutForSerialization()
        {
            return new MemoryInjectionException(InjectionType.CodeAndVariableInjection, "aq", new Exception("foo"));
        }

        protected override IEnumerable<PropertyInfo> GetExpectedPropertiesForSerialization()
        {
            yield return GetPropertyFromExpression(e => e.InjectionType);
            yield return GetPropertyFromExpression(e => e.DetectionTime);
            yield return GetPropertyFromExpression(e => e.InjectedObject);
        }
    }
}
#endif