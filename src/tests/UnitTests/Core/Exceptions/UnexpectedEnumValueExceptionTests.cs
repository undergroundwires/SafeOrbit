using NUnit.Framework;
#if !NETCOREAPP1_1
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Exceptions
{
    [TestFixture]
    public class UnexpectedEnumValueExceptionTests : SerializableExceptionTestsBase<
        UnexpectedEnumValueException<UnexpectedEnumValueExceptionTests.TestEnum>>
    {
        protected override UnexpectedEnumValueException<TestEnum> GetSutForSerialization()
        {
            return new UnexpectedEnumValueException<TestEnum>(TestEnum.Val1);
        }

        protected override IEnumerable<PropertyInfo> GetExpectedPropertiesForSerialization()
        {
            yield return GetPropertyFromExpression(e => e.Value);
        }

        public enum TestEnum
        {
            Val1,
            Val2
        }
    }
}
#endif