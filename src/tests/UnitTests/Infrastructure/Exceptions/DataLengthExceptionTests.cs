#if !NETCOREAPP1_1
using NUnit.Framework;

namespace SafeOrbit.Exceptions
{
    /// <seealso cref="DataLengthException" />
    [TestFixture]
    public class DataLengthExceptionTests : SerializableExceptionTestsBase<DataLengthException>
    {
        protected override DataLengthException GetSutForSerialization() => new DataLengthException("argumentName", "message");
    }
}
#endif