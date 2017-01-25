using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    public static class AppSettingsReaderExtension
    {
        #region System.Configuration.AppSettingsReader
        /// <summary>
        /// AppSettingsより指定のキー値を取得します。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="self">System.Configuration.AppSettingsReader</param>
        /// <param name="key">キー</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this AppSettingsReader self, string key, ref T value)
        {
            try
            {
                value = self.GetValue<T>(key);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// AppSettingsより指定のキー値を取得します。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="self">System.Configuration.AppSettingsReader</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this AppSettingsReader self, string key)
        {
            return (T)Convert.ChangeType(self.GetValue(key, typeof(T)), typeof(T));
        }
        #endregion
    }
}
