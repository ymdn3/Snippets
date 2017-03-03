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

        /// <summary>
        /// Derived pattern converters must override this method in order to convert conversion specifiers in the correct way.
        /// </summary>
        /// <param name="writer">System.IO.TextWriter that will receive the formatted result.</param>
        /// <param name="loggingEvent">The log4net.Core.LoggingEvent on which the pattern converter should be executed.</param>
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var ipaddr = "0.0.0.0";
            var context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    if (context.Request != null)
                        ipaddr = ConvertToIPv4(context.Request.UserHostAddress);
                }
                catch (HttpException)
                {
                    //NOOP
                }
                catch (Exception e)
                {
                    ipaddr = e.GetType().Name;
                }
            }
            writer.Write(ipaddr);
        }

        private string ConvertToIPv4(string addr)
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
