using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 指定のオブジェクトからキーバリュー型のDictionaryを生成します。
        /// ネストオブジェクトはプロパティ名を連結し、フラットな状態に変換します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="separator">プロパティ名連結セパレータ</param>
        /// <returns></returns>
        public static IDictionary<string, object> ToFlattenDictionary(this object self, string separator)
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            dict.AddKeyValue(self, separator);
            return dict;
        }

    }
}
