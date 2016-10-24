namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    using System.Collections.Generic;
    internal interface IInstanceProviderRule
    {
        bool IsSatisfiedBy(IInstanceProvider instance);
        IEnumerable<string> Errors { get; }
        RuleType Type { get; }
    }
}
