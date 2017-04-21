using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class CollectionsExtension
    {
        #region System.Collections.Generic.IList
        /// <summary>
        /// http://stackoverflow.com/questions/11981282/convert-json-to-datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static System.Data.DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        #endregion

        #region System.Collections.Generic.IDictionary
        /// <summary>
        /// Dynamicオブジェクト(new {})をDictionaryに追加します。
        /// ネストオブジェクトはプロパティ名を連結し、フラットな状態に変換します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value">dynamicオブジェクト</param>
        public static void AddKeyValue(this IDictionary<string, object> self, object value)
        {
            if (value != null)
                AddKeyValue(self, null, value, null);
        }
        /// <summary>
        /// Dynamicオブジェクト(new {})をDictionaryに追加します。
        /// ネストオブジェクトはプロパティ名を連結し、フラットな状態に変換します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value">dynamicオブジェクト</param>
        /// <param name="nameseparator">プロパティ名連結セパレータ</param>
        public static void AddKeyValue(this IDictionary<string, object> self, object value, string nameseparator)
        {
            if (value != null)
                AddKeyValue(self, null, value, nameseparator);
        }
        /// <summary>
        /// Dynamicオブジェクト(new {})をDictionaryに追加します。
        /// ネストオブジェクトはプロパティ名を連結し、フラットな状態に変換します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name">ネスト親のプロパティ名</param>
        /// <param name="value">dynamicオブジェクト</param>
        /// <param name="nameseparator">プロパティ名連結セパレータ</param>
        public static void AddKeyValue(this IDictionary<string, object> self, string name, object value, string nameseparator)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(value);
            if (props.Count > 0)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    object val = prop.GetValue(value);
                    string n = string.IsNullOrEmpty(name) ? prop.Name : name + nameseparator + prop.Name;
                    //値がnullか値型か文字列なら上書きで追加
                    if (val == null || prop.PropertyType.IsValueType || val.GetType() == typeof(string))
                    {
                        if (self.ContainsKey(n))
                            self.Remove(n);
                        self.Add(n, val);
                    }
                    else
                    {
                        //下層へ
                        AddKeyValue(self, n, val, nameseparator);
                    }
                }
            }
            else
            {
                self.Add(name, value);
            }
        }
        #endregion

        #region System.Collections.Generic.IEnumerable
        /// <summary>
        /// このインスタンスの列挙子すべてに、指定の処理を行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// このインスタンスの列挙子すべてに、指定の処理を行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task EachAsync<T>(this IEnumerable<T> source, Action<T> action, bool configureAwait)
        {
            var tasks = source.Select(x => Task.Run(() => action(x)));
            await tasks.WhenAll().ConfigureAwait(configureAwait);
        }

        /// <summary>
        /// 指定した要素を最後に追加したシーケンスを返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new[] { item });
        }

        /// <summary>
        /// 指定した要素を先頭に追加したシーケンスを返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            return (new[] { item }).Concat(source);
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

        /// <summary>
        /// 指定されたすべてのタスクが完了してから完了するタスクを作成します。
        /// </summary>
        /// <param name="tasks">完了を待機するタスク。</param>
        /// <returns>指定されたすべてのタスクの完了を表すタスク。</returns>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// 指定されたすべてのタスクが完了してから完了するタスクを作成します。
        /// </summary>
        /// <typeparam name="T">完了したタスクの型。</typeparam>
        /// <param name="tasks">完了を待機するタスク。</param>
        /// <returns>指定されたすべてのタスクの完了を表すタスク。</returns>
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return Task.WhenAll(tasks);
        }
        #endregion
    }
}
