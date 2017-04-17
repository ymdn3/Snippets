using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net.Core;
using log4net.Layout.Pattern;

namespace KOILib.Common.Log4.Pattern
{
    /// <summary>
    /// Log4netの出力パターンを、現コンテキストのリモートIPアドレスに変換する処理を提供します。
    /// </summary>
    public class HttpRemoteIP
        : PatternLayoutConverter
    {
        private Dictionary<string, string> _convertCache;

        /// <summary>
        /// Derived pattern converters must override this method in order to convert conversion specifiers in the correct way.
        /// </summary>
        /// <param name="writer">System.IO.TextWriter that will receive the formatted result.</param>
        /// <param name="loggingEvent">The log4net.Core.LoggingEvent on which the pattern converter should be executed.</param>
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            const string unknown = "0.0.0.0";
            var ipaddr = unknown;
            var context = HttpContext.Current;
            if (context != null)
            {
                if (_convertCache == null)
                    _convertCache = new Dictionary<string, string>();

                var hostaddr = default(string);
                try
                {
                    if (context.Request != null)
                        hostaddr = context.Request.UserHostAddress;

                    if (_convertCache.ContainsKey(hostaddr))
                    {
                        ipaddr = _convertCache[hostaddr];
                    }
                    else
                    {
                        try
                        {
                            ipaddr = ConvertToIPv4(hostaddr);
                        }
                        catch (Exception e)
                        {
                            ipaddr = e.GetType().Name;
                        }
                        _convertCache.Add(hostaddr, ipaddr);
                    }
                }
                catch (HttpException)
                {
                    //NOOP
                }
            }
            writer.Write(ipaddr);
        }

        private string ConvertToIPv4(string addr)
        {
            var iphEntry = Dns.GetHostEntry(addr);
            var ipv4 = iphEntry.AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            if (ipv4 == default(IPAddress))
                return addr;
            else
                return ipv4.ToString();
        }

    }
}
