﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static IDictionary<string, object> DictionaryFrom(object values)
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            if (values != null)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor prop in props)
                {
                    object val = prop.GetValue(values);
                    dict.Add(prop.Name, val);
                }
            }
            return dict;
        }

    }
}
