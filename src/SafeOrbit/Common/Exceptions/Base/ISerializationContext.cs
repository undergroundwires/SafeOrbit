using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SafeOrbit.Exceptions
{
    public interface ISerializationContext
    {
        ICollection<ISerializationPropertyInfo> PropertyInfos { get; }
        void Add<T>(Expression<Func<T>> property);
    }
}