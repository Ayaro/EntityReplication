using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityReplication
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<MemberInfo> GetPropertiesExceptBinded(this Type type, IEnumerable<MemberBinding> bindings)
        {
            IEnumerable<MemberInfo> bindedProperties = bindings.Select(x => x.Member);

            return type.GetProperties().Except(bindedProperties);
        } 
    }
}