using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    /// <summary>
    /// StringListDecoration 括り文字の開始終了位置
    /// </summary>
    [Flags]
    public enum StringListQuotePositions
    {
        /// <summary>
        /// 括りなし
        /// </summary>
        None = 0,
        /// <summary>
        /// 開始側
        /// </summary>
        Pre = 0x1,
        /// <summary>
        /// 終了側
        /// </summary>
        Post = 0x2,
        /// <summary>
        /// 開始・終了とも
        /// </summary>
        Both = 0x3,
    }

    /// <summary>
    /// StringList.ToString()デコレーションクラス
    /// </summary>
    public class StringListDecoration
    {
        #region Static Members
        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        /// <returns></returns>
        public static StringListDecoration From()
        {
            return new StringListDecoration();
        }

        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <returns></returns>
        public static StringListDecoration From(string sep)
        {
            if (sep == null)
                return From();

            return new StringListDecoration
            {
                Delimiter = sep
            };
        }
        
        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="quot">括り文字</param>
        /// <param name="quotpos">括り文字の位置</param>
        /// <returns></returns>
        public static StringListDecoration From(string sep, char quot, StringListQuotePositions quotpos)
        {
            return new StringListDecoration
            {
                Delimiter = sep,
                PreQuote = quotpos.HasFlag(StringListQuotePositions.Pre) ? quot : default(char),
                PostQuote = quotpos.HasFlag(StringListQuotePositions.Post) ? quot : default(char),
            };
        }

        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="prequot">前側の括り文字</param>
        /// <param name="postquot">後側の括り文字</param>
        /// <returns></returns>
        public static StringListDecoration From(string sep, char prequot, char postquot)
        {
            return new StringListDecoration
            {
                Delimiter = sep,
                PreQuote = prequot,
                PostQuote = postquot,
            };
        }

        /// <summary>
        /// インスタンスを生成する。
        /// （括り文字を、前後とも一括で指定する）
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="quot">1文字の場合は前後とも同じ文字</param>
        /// <returns></returns>
        public static StringListDecoration From(string sep, string quot)
        {
            if (quot == null)
                return From(sep);

            if (quot.Length == 0)
                return From(sep);
            else if (quot.Length == 1)
                return From(sep, quot[0], quot[0]);
            else
                return From(sep, quot[0], quot[1]);
        }
        #endregion

        
        /// <summary>
        /// 区切り文字
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// 前側の括り文字。
        /// 不要な場合は default(char) または \0
        /// </summary>
        public char PreQuote { get; set; }

        /// <summary>
        /// 後側の括り文字。
        /// 不要な場合は default(char) または \0
        /// </summary>
        public char PostQuote { get; set; }

        /// <summary>
        /// 終端に改行文字を付与するかどうか
        /// </summary>
        public bool EndLineFeed { get; set; }

        /// <summary>
        /// 区切り文字が未指定かどうか
        /// </summary>
        public bool DelimiterIsEmpty
        {
            get { return string.IsNullOrEmpty(Delimiter); }
        }

        /// <summary>
        /// 前側の括り文字が未指定かどうか
        /// </summary>
        public bool PreQuoteIsEmpty
        {
            get { return (PreQuote == default(char)); }
        }

        /// <summary>
        /// 後側の括り文字が未指定かどうか
        /// </summary>
        public bool PostQuoteIsEmpty
        {
            get { return (PostQuote == default(char)); }
        }

        #region Constructors
        /// <summary>
        /// 非公開コンストラクタ
        /// </summary>
        private StringListDecoration()
        {
        }
        #endregion
    }
}
