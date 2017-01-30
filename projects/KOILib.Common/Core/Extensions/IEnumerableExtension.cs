using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    public static class IEnumerableExtension
    {
        #region System.Collections.Generic.IEnumerable
        /// <summary>
        /// このインスタンスの列挙子すべてに、指定の処理を行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void Do<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
                action(item);
        }

        /// <summary>
        /// このインスタンスの列挙子すべてに、指定の処理を並列で行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void ParallelDo<T>(this IEnumerable<T> self, Action<T> action)
        {
            Parallel.ForEach(self, action);
        }
        #endregion
    }
}
