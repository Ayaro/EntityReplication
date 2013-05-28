using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityReplication.PrototypesStorage
{
    internal class SingleBindingPerPropertyComparer : IEqualityComparer<MemberBinding>
    {
        public bool Equals(MemberBinding x, MemberBinding y)
        {
            return Equals(x.Member, y.Member);
        }

        public int GetHashCode(MemberBinding obj)
        {
            return (obj == null) ? 0 : obj.Member.GetHashCode();
        }
    }
}