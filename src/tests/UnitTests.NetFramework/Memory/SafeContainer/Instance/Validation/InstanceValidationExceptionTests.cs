using System;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    /// <seealso cref="SerializableExceptionTestsBase{InstanceValidationException}" />
    /// <seealso cref="InstanceValidationException" />
    public class InstanceValidationExceptionTests : SerializableExceptionTestsBase<InstanceValidationException>
    {
        protected override InstanceValidationException GetSutForSerialization()
            => new InstanceValidationException("message", new Exception("foo"));
    }
}