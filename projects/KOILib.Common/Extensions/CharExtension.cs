using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class CharExtension
    {
        #region System.Char
        /// <summary>
        /// 指定の文字 c のみで構成された長さ len の文字列を生成します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Repeat(this char self, int len)
        {
            return new String(self, len);
        }
        #endregion

    }
}
