
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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