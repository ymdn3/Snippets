using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        public static bool EqualsAs<T>(this T self, string another)
            where T : struct
        {
            return self.ToString().Equals(another, StringComparison.Ordinal);
        }

        #endregion

        public static string ConvertToIPv4(string addr)
        {
            var iphEntry = Dns.GetHostEntry(addr);
            var ipv4 = iphEntry.AddressList
                .FirstOrDefault((entry) => entry.AddressFamily == AddressFamily.InterNetwork);
            if (ipv4 == default(IPAddress))
            {
                return addr;
            }
            else
            {
                return ipv4.ToString();
            }
        }

    }
}
