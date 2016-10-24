using System;

namespace SafeOrbit.Memory.Injection
{
    public class InjectionMessage : IInjectionMessage
    {
        public InjectionType InjectionType { get; }
        public object InjectedObject { get; }
        public DateTimeOffset InjectionDetectionTime { get; }
        public InjectionMessage(bool isStateInjected, bool isCodeInjected, object injectedObject)
        {
            InjectionType = GetInjectionType(isStateInjected, isCodeInjected);
            InjectedObject = injectedObject;
            InjectionDetectionTime = DateTimeOffset.UtcNow;
        }
        private InjectionType GetInjectionType(bool isStateInjected, bool isCodeInjected)
        {
            if (isStateInjected && isCodeInjected) return InjectionType.CodeAndVariableInjection;
            if (isStateInjected) return InjectionType.VariableInjection;
            if (isCodeInjected) return InjectionType.CodeInjection;
            throw new ArgumentException("There is no injection : Neither state or code is injected.");
        }

        public override string ToString() => $"{nameof(InjectionType)} = \"{InjectionType}\", {nameof(InjectionDetectionTime)} = \"{InjectionDetectionTime.ToLocalTime()}\"";


        #region Equality
        public bool Equals(IInjectionMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            return this.InjectionType.Equals(other.InjectionType)
                && (this.InjectedObject?.Equals(other.InjectedObject) ?? false)
                && this.InjectionDetectionTime.Equals(other.InjectionDetectionTime);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return -590645308
                    * (int)this.InjectionType
                    * this.InjectionDetectionTime.GetHashCode()
                    * (this.InjectedObject?.GetHashCode() ?? 1);
            }
        }
        public override bool Equals(object obj) => this.Equals(obj as IInjectionMessage);
        public static bool operator ==(InjectionMessage a, InjectionMessage b) => ReferenceEquals(null, a) ? ReferenceEquals(null, b) : a.Equals(b);
        public static bool operator !=(InjectionMessage a, InjectionMessage b) => !(a == b);
        #endregion
    }
}