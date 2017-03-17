﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class StringExtension
    {

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
            return values.Any(value => self.Equals(value, comparisonType));
        }

        /// <summary>
        /// この文字列と、指定した文字列のいずれかと値が同一かどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string self, IEnumerable<string> values)
        {
            return values.Any(value => self.Equals(value));
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
            return values.Any(value => self.StartsWith(value, comparisonType));
        }

        /// <summary>
        /// この文字列インスタンスの先頭が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string self, IEnumerable<string> values)
        {
            return values.Any(value => self.StartsWith(value));
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
            return values.Any(value => self.EndsWith(value, comparisonType));
        }

        /// <summary>
        /// この文字列インスタンスの末尾が、指定した文字列のいずれかに一致するかどうかを判断します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string self, IEnumerable<string> values)
        {
            return values.Any(value => self.EndsWith(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Slice(this string self, int start, int end)
        {
            if (start < 0) start = self.Length + start;
            if (end < 0) end = self.Length + end;
            return self.Substring(start, end - start);
        }

        /// <summary>
        /// 
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
        public static string[] SplitByCommaOrFeed(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
                return new string[] { };

            //空白とタブの削除
            var value = System.Text.RegularExpressions.Regex.Replace(self, @"[ 　\t]", "", System.Text.RegularExpressions.RegexOptions.Compiled);
            //改行・カンマ分離
            var values = value.Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return values;
        }
        #endregion

    }
}