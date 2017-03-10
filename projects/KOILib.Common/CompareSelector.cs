using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    public class CompareSelector<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> _selector;

        public bool Equals(T x, T y)
        {
            return _selector(x).Equals(_selector(y));
        }

        public int GetHashCode(T obj)
        {
            return _selector(obj).GetHashCode();
        }

        public CompareSelector(Func<T, TKey> selector)
        {
            _selector = selector;
        }
    }
}
