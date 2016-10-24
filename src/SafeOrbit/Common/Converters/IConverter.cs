namespace SafeOrbit.Converters
{
    public interface IConverter<in TInput, out TSource>
    {
        TSource Convert(TInput input);
    }
}
