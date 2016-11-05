
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal class SingletonShouldNotImplementIDisposable : IInstanceProviderRule
    {
        public IEnumerable<string> Errors { get; private set; }
        public RuleType Type { get; } = RuleType.Warning;
        public bool IsSatisfiedBy(IInstanceProvider instance)
        {
            if (instance.LifeTime == LifeTime.Transient) return true;
            var type = instance.ImplementationType;
            if (type == null) throw new ArgumentException($"Type ({type.FullName}) could not be loaded");
            var isDisposable = typeof(IDisposable).GetTypeInfo().IsAssignableFrom(type);
            if (!isDisposable) return true;
            Errors = new[] { $"{type.AssemblyQualifiedName} implements {nameof(IDisposable)} but registered as {nameof(LifeTime.Singleton)}" };
            return false;
        }
    }
}