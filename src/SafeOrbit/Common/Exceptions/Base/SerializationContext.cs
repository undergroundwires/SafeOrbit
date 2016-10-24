using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SafeOrbit.Exceptions
{
    public class SerializationContext : ISerializationContext
    {
        public ICollection<ISerializationPropertyInfo> PropertyInfos { get; } = new List<ISerializationPropertyInfo>();

        public void Add<T>(Expression<Func<T>> property)
        {
            var name = GetName(property);
            var value = property.Compile()();

            PropertyInfos.Add(new SerializationPropertyInfo(
                value: value,
                propertyName: name,
                type: typeof(T)
                ));
        }

        private static string GetName<T>(Expression<Func<T>> exp)
        {
            var body = exp.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression) exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}