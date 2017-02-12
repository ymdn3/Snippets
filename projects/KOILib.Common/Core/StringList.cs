using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core
{
    /// <summary>
    /// string型List 拡張クラス
    /// </summary>
    public class StringList
        : List<string>
    {
        /// <summary>
        /// ToStringデコレーション情報
        /// </summary>
        public StringListDecoration DecorateInfo { get; set; }

        #region Methods
        /// <summary>
        /// 引数の ToString() の結果を Add します
        /// </summary>
        /// <param name="@object"></param>
        /// <returns></returns>
        public StringList Add(object @object)
        {
            if (@object is string)
                base.Add((string)@object); 
            else
                base.Add(@object.ToString());
            return this;
        }
        /// <summary>
        /// 書式入力インタフェース
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public StringList Add(string format, params object[] args)
        {
            if (format != null)
            {
                if (args.Length > 0)
                    base.Add(string.Format(format, args));
                else
                    base.Add(format);
            }
            return this;
        }
        /// <summary>
        /// 書式入力インタフェース
        /// </summary>
        /// <param name="index"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public StringList Insert(int index, string format, params object[] args)
        {
            if (format != null)
            {
                if (args.Length > 0)
                    base.Insert(index, string.Format(format, args));
                else
                    base.Insert(index, format);
            }
            return this;
        }
        /// <summary>
        /// Range入力インタフェース
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public StringList AddRange(IEnumerable<string> collection, params object[] args)
        {
            foreach (var item in collection) this.Add(item, args);
            return this;
        }
        /// <summary>
        /// Range入力インタフェース
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public StringList InsertRange(int index, IEnumerable<string> collection, params object[] args)
        {
            base.InsertRange(index, (new StringList().AddRange(collection, args)));
            return this;
        }
        
        /// <summary>
        /// 長さ 0 の文字列を Add します
        /// </summary>
        /// <returns></returns>
        public StringList AddEmpty()
        {
            return this.Add("");
        }

        /// <summary>
        /// Environment.NewLine を Add します
        /// </summary>
        /// <returns></returns>
        public StringList AddLine()
        {
            return this.Add(Environment.NewLine);
        }

        /// <summary>
        /// 長さ 0 の文字列が格納されている要素をすべて削除します
        /// </summary>
        /// <param name="withWhitespace">trueのとき、空白文字も削除対象とします</param>
        /// <returns></returns>
        public StringList RemoveEmpty(bool withWhitespace)
        {
            if (withWhitespace)
                this.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            else
                this.RemoveAll(s => string.IsNullOrEmpty(s));
            return this;
        }

        /// <summary>
        /// ToStringで出力する、区切り文字と括り文字を指定します。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="prequot">括り文字（開始）</param>
        /// <param name="postquot">括り文字（終了）</param>
        /// <returns></returns>
        public StringList Decorate(string sep, char prequot, char postquot)
        {
            DecorateInfo = StringListDecoration.From(sep, prequot, postquot);
            return this;
        }

        /// <summary>
        /// ToStringで出力する、区切り文字と括り文字を指定します。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="quot">括り文字</param>
        /// <returns></returns>
        public StringList Decorate(string sep, char quot, StringListQuotePositions pos)
        {
            DecorateInfo = StringListDecoration.From(sep, quot, pos);
            return this;
        }

        /// <summary>
        /// ToStringで出力する、区切り文字と括り文字を指定します。
        /// </summary>
        /// <param name="sep">区切り文字</param>
        /// <param name="quot">括り文字（はじめの2文字を開始文字と終了文字とに分割します）</param>
        /// <returns></returns>
        public StringList Decorate(string sep = null, string quot = null)
        {
            DecorateInfo = StringListDecoration.From(sep, quot);
            return this;
        }

        [Obsolete(error: true, message: "→ Decorate().ToString();")]
        public string ToString(string sep, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// インスタンスが保持するデコレーション情報を使って string を生成します
        /// </summary>
        /// <param name="args">指定された場合、string.Format() の引数として引き渡されます</param>
        /// <returns></returns>
        public string ToString(params object[] args)
        {
            var delimiterIsEmpty = DecorateInfo.DelimiterIsEmpty;
            var preQuotIsEmpty = DecorateInfo.PreQuoteIsEmpty;
            var postQuotIsEmpty = DecorateInfo.PostQuoteIsEmpty;

            var sb = new StringBuilder();
            for (var i = 0; i < this.Count; i++)
            {
                //skip null
                if (this[i] == null) continue;

                if (!delimiterIsEmpty && sb.Length > 0)
                    sb.Append(DecorateInfo.Delimiter);

                if (!preQuotIsEmpty)
                    sb.Append(DecorateInfo.PreQuote);

                sb.Append(this[i]);

                if (!postQuotIsEmpty)
                    sb.Append(DecorateInfo.PostQuote);
            }

            if (DecorateInfo.EndLineFeed)
                sb.Append(Environment.NewLine);

            if (args.Length > 0)
                return string.Format(sb.ToString(), args);
            else
                return sb.ToString();
        }

        /// <summary>
        /// string を生成します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            DecorateInfo = DecorateInfo ?? StringListDecoration.From();
            return ToString(DecorateInfo);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public StringList()
        {
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="item">初期追加文字列</param>
        public StringList(string item)
            : base()
        {
            if (!string.IsNullOrEmpty(item)) base.Add(item);
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="items">初期追加文字列配列</param>
        public StringList(params string[] items)
            : base(items)
        {
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="collection">初期追加するIEnumerableの文字列コレクション</param>
        public StringList(IEnumerable<string> collection)
            : base(collection)
        {
        }
        #endregion
    }
}
