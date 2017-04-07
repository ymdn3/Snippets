using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common.Extensions;

namespace KOILib.Common
{
    /// <summary>
    /// 分類できていないユーティリティ関数群。
    /// いずれ適切なクラスに移動する予定。
    /// </summary>
    public static class Utils
    {
        #region Extensions
        public static bool EqualsAny<T>(this T self, IEnumerable<T> values)
            where T : struct
        {
            return values.Any(value => self.Equals(value));
        }
        #endregion

        /// <summary>
        /// 指定のオブジェクトからキーバリュー型のDictionaryを生成します。
        /// ネストオブジェクトはプロパティ名を連結し、フラットな状態に変換します。
        /// </summary>
        /// <param name="object"></param>
        /// <param name="separator">プロパティ名連結セパレータ</param>
        /// <returns></returns>
        public static IDictionary<string, object> ToFlattenDictionary(dynamic @object, string separator)
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            dict.AddKeyValue((object)@object, separator);
            return dict;
        }

        public static string ConvertToIPv4(string addr)
        {
            var iphEntry = Dns.GetHostEntry(addr);
            var ipv4 = iphEntry.AddressList
                .FirstOrDefault((entry) => entry.AddressFamily == AddressFamily.InterNetwork);
            if (ipv4 == default(IPAddress))
                return addr;
            else
                return ipv4.ToString();
        }

    }
}
