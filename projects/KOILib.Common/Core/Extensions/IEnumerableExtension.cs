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
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// このインスタンスの列挙子すべてに、指定の処理を並列で行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ParallelDo<T>(this IEnumerable<T> source, Action<T> action)
        {
            Parallel.ForEach(source, action);
        }

        /// <summary>
        /// 指定したセレクターを使用して、指定した要素がシーケンスに含まれているかどうかを判断します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source">値の検索対象となるシーケンス。</param>
        /// <param name="value">シーケンス内で検索する値。</param>
        /// <param name="selector">値を比較するセレクター</param>
        /// <returns></returns>
        public static bool Contains<T, TKey>(this IEnumerable<T> source, T value, Func<T, TKey> selector)
        {
            return source.Contains(value, new CompareSelector<T, TKey>(selector));
        }

        /// <summary>
        /// 指定されたセレクターを使用して値を比較することにより、シーケンスから一意の要素を返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source">重複する要素を削除する対象となるシーケンス。</param>
        /// <param name="selector">値を比較するセレクター</param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }

        /// <summary>
        /// 指定されたセレクターを使用して値を比較することにより、2 つのシーケンスの差集合を生成します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first">second には含まれていないが、返される要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="second">最初のシーケンスにも含まれ、返されたシーケンスからは削除される要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="selector">値を比較するセレクター</param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> selector)
        {
            return first.Except(second, new CompareSelector<T, TKey>(selector));
        }

        /// <summary>
        /// 指定されたセレクターを使用して値を比較することにより、2 つのシーケンスの積集合を生成します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first">second にも含まれる、返される一意の要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="second">最初のシーケンスにも含まれる、返される一意の要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="selector">値を比較するセレクター</param>
        /// <returns></returns>
        public static IEnumerable<T> Intersect<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> selector)
        {
            return first.Intersect(second, new CompareSelector<T, TKey>(selector));
        }

        /// <summary>
        /// 指定された System.Collections.Generic.IEqualityComparer`1 を使用して 2 つのシーケンスの和集合を生成します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first">和集合の最初のセットを形成する一意の要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="second">和集合の 2 番目のセットを形成する一意の要素を含む System.Collections.Generic.IEnumerable`1。</param>
        /// <param name="selector">値を比較するセレクター</param>
        /// <returns></returns>
        public static IEnumerable<T> Union<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> selector)
        {
            return first.Union(second, new CompareSelector<T, TKey>(selector));
        }
        #endregion
    }
}
