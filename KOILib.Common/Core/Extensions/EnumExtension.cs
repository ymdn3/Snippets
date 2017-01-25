using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    /// <summary>
    /// enum 型拡張メソッド
    /// </summary>
    public static class EnumExtension
    {
        #region System.Enum
        /// <summary>
        /// 指定した形式に従って、指定した列挙型の指定した値をそれと等価の文字列形式に変換します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="fmt">使用する出力書式。G|X|D|F</param>
        /// <returns></returns>
        public static string Format(this Enum self, string fmt)
        {
            return Enum.Format(self.GetType(), self, fmt);
        }
        /// <summary>
        /// enum値の名前を取得します。
        /// </summary>
        /// <param name="self"></param>
        /// <returns>値が e である列挙体にある列挙型定数の名前が含まれた文字列。そのような定数が見つからない場合は、null 参照 (Visual Basic では Nothing)。</returns>
        public static string ToName(this Enum self)
        {
            return Enum.GetName(self.GetType(), self);
        }
        /// <summary>
        /// enum値を、列挙値の基になる型(UnderingType)であらわされる値に変換します
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static object ToValue(this Enum self)
        {
            var t = Enum.GetUnderlyingType(self.GetType());
            return Convert.ChangeType(self, Type.GetTypeCode(t));
        }
        #endregion
    }
}
