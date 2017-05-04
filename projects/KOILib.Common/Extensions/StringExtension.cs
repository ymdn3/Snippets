using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class StringExtension
    {
        [DllImport("kernel32.dll")]
        extern private static int LCMapStringW(int locale, uint dwMapFlags, [MarshalAs(UnmanagedType.LPWStr)]string lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.LPWStr)]string lpDestStr, int cchDest);
        private enum dwMapFlags: uint
        {
            NORM_IGNORECASE = 0x00000001,           //大文字と小文字を区別しません。
            NORM_IGNORENONSPACE = 0x00000002,       //送りなし文字を無視します。このフラグをセットすると、日本語アクセント文字も削除されます。
            NORM_IGNORESYMBOLS = 0x00000004,        //記号を無視します。
            LCMAP_LOWERCASE = 0x00000100,           //小文字を使います。
            LCMAP_UPPERCASE = 0x00000200,           //大文字を使います。
            LCMAP_SORTKEY = 0x00000400,             //正規化されたワイド文字並び替えキーを作成します。
            LCMAP_BYTEREV = 0x00000800,             //Windows NT のみ : バイト順序を反転します。たとえば 0x3450 と 0x4822 を渡すと、結果は 0x5034 と 0x2248 になります。
            SORT_STRINGSORT = 0x00001000,           //区切り記号を記号と同じものとして扱います。
            NORM_IGNOREKANATYPE = 0x00010000,       //ひらがなとカタカナを区別しません。ひらがなとカタカナを同じと見なします。
            NORM_IGNOREWIDTH = 0x00020000,          //シングルバイト文字と、ダブルバイトの同じ文字とを区別しません。
            LCMAP_HIRAGANA = 0x00100000,            //ひらがなにします。
            LCMAP_KATAKANA = 0x00200000,            //カタカナにします。
            LCMAP_HALFWIDTH = 0x00400000,           //半角文字にします（適用される場合）。
            LCMAP_FULLWIDTH = 0x00800000,           //全角文字にします（適用される場合）。
            LCMAP_LINGUISTIC_CASING = 0x01000000,   //大文字と小文字の区別に、ファイルシステムの規則（既定値）ではなく、言語上の規則を使います。LCMAP_LOWERCASE、または LCMAP_UPPERCASE とのみ組み合わせて使えます。
            LCMAP_SIMPLIFIED_CHINESE = 0x02000000,  //中国語の簡体字を繁体字にマップします。
            LCMAP_TRADITIONAL_CHINESE = 0x04000000, //中国語の繁体字を簡体字にマップします。
        }

        #region System.String

        /// <summary>
        /// この文字列と、指定した文字列のいずれかと値が同一かどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string self, IEnumerable<string> values, StringComparison comparisonType)
        {
            return values.Any(x => self.Equals(x, comparisonType));
        }

        /// <summary>
        /// この文字列と、指定した文字列のいずれかと値が同一かどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string self, IEnumerable<string> values)
        {
            return values.Any(x => self.Equals(x));
        }

        /// <summary>
        /// この文字列インスタンスの先頭が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string self, IEnumerable<string> values, StringComparison comparisonType)
        {
            return values.Any(x => self.StartsWith(x, comparisonType));
        }

        /// <summary>
        /// この文字列インスタンスの先頭が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string self, IEnumerable<string> values)
        {
            return values.Any(x => self.StartsWith(x));
        }

        /// <summary>
        /// この文字列インスタンスの末尾が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string self, IEnumerable<string> values, StringComparison comparisonType)
        {
            return values.Any(x => self.EndsWith(x, comparisonType));
        }

        /// <summary>
        /// この文字列インスタンスの末尾が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string self, IEnumerable<string> values)
        {
            return values.Any(x => self.EndsWith(x));
        }

        /// <summary>
        /// start と end で指定した位置の範囲の文字列を返します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Slice(this string self, int start, int end)
        {
            if (start < 0) start = self.Length + start;
            if (end < 0) end = self.Length + end;
            return self.Substring(start, Math.Min(self.Length, end - start));
        }

        /// <summary>
        /// start で指定した位置以後の文字列を返します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static string Slice(this string self, int start)
        {
            return Slice(self, start, self.Length);
        }

        /// <summary>
        /// 指定のエンコーディングで指定されたバイト長に収まるよう、文字列を左から切り出します。
        /// </summary>
        /// <param name="org"></param>
        /// <param name="size">バイト長</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns></returns>
        public static string LeftB(this string self, int size, Encoding enc)
        {
            if (size <= 0) return "";

            var bytes = enc.GetBytes(self);

            //指定のサイズが元文字列の長さ以上の場合は、元の文字列をそのまま返す
            if (size >= bytes.Length) return self;

            //指定のサイズで文字列を復元
            var t = enc.GetString(bytes, 0, size);

            //最後の文字が正しく復元できない場合は切り捨てる
            if (t.Substring(t.Length - 1) != self.Substring(t.Length - 1, 1))
            {
                t = t.Substring(0, t.Length - 1);
            }

            return t;
        }

        /// <summary>
        /// 文字列が指定のエンコーディングで指定されたバイト長をオーバーする場合、Trueを返す。
        /// </summary>
        /// <param name="s"></param>
        /// <param name="size">バイト長</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns></returns>
        public static bool IsOverflowB(this string self, int size, Encoding enc)
        {
            return enc.GetByteCount(self) > size;
        }

        /// <summary>
        /// 文字列が指定のエンコーディングでエンコードできる場合、Trueを返す
        /// </summary>
        /// <param name="self"></param>
        /// <param name="enc">エンコーディング</param>
        /// <returns></returns>
        public static bool IsEncodable(this string self, Encoding enc)
        {
            var cloneEnc = (Encoding)enc.Clone();
            cloneEnc.EncoderFallback = new EncoderExceptionFallback();
            try
            {
                cloneEnc.GetBytes(self);
                return true;
            }
            catch (EncoderFallbackException)
            {
                return false;
            }
        }

        /// <summary>
        /// 空白を詰める
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string CloseWhitespace(this string self)
        {
            return System.Text.RegularExpressions.Regex.Replace(self, @" * ('[^']*'|[^ ])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        /// <summary>
        /// 文字列値を、カンマまたは改行区切りとした配列に変換します。
        /// ホワイトスペース(全半角スペースとタブ文字)はサプレスされ、空文字列の配列要素は結果から除外されます。
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string[] SplitByCommaOrFeed(this string self, bool remainEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(self))
                return new string[] { };


            //空白とタブの削除
            var value = System.Text.RegularExpressions.Regex.Replace(self, @"[ 　\t]", "", System.Text.RegularExpressions.RegexOptions.Compiled);
            //改行・カンマ分離
            var option = remainEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;
            var values = value.Split(new char[] { '\r', '\n', ',' }, option);
            return values;
        }

        /// <summary>
        /// 文字列変換
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static string ConvertMap(this string self, dwMapFlags flags)
        {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            var result = ' '.Repeat(self.Length);
            LCMapStringW(ci.LCID, (uint)flags, self, self.Length, result, result.Length);
            return result;
        }

        /// <summary>
        /// 半角を全角に変換します
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToFullWidth(this string self)
        {
            return ConvertMap(self, dwMapFlags.LCMAP_FULLWIDTH);
        }

        /// <summary>
        /// 全角を半角に変換します
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToHalfWidth(this string self)
        {
            return ConvertMap(self, dwMapFlags.LCMAP_HALFWIDTH | dwMapFlags.LCMAP_KATAKANA);
        }
        #endregion

    }
}
