using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// Microsoft Excel シートデータを列数固定の帳票形式とみなして読み取るクラスを表します。
    /// </summary>
    public class ExcelRowReader
        : IDataReader, IDisposable
    {
        #region Fields
        /// <summary>
        /// 解放が必要なCOMオブジェクトコレクション
        /// </summary>
        private ComReleaser _ComObjects = new ComReleaser();

        /// <summary>
        /// ExcelOperatorオブジェクト
        /// </summary>
        private ExcelOperator _XlOperator = null;

        /// <summary>
        /// 対象シートオブジェクト
        /// </summary>
        private Worksheet _Sheet = null;

        /// <summary>
        /// リーダーの列数
        /// </summary>
        private int _ColumnCount = 0;

        /// <summary>
        /// 列オフセット
        /// </summary>
        private int _ColumnOffset = 0;

        /// <summary>
        /// 対象シートの最終行インデックス
        /// </summary>
        private int _LastIndex = 0;

        /// <summary>
        /// 読み込み中の行インデックス
        /// </summary>
        private int _CurrentIndex = 0;

        /// <summary>
        /// 読み込み中の行オブジェクト
        /// </summary>
        private Range _CurrentRow = null;

        /// <summary>
        /// EOF
        /// </summary>
        private bool _EOF = false;

        /// <summary>
        /// Values(Range.Valueプロパティ)
        /// </summary>
        private List<object> _Values = null;

        /// <summary>
        /// Texts(Range.Textプロパティ)
        /// </summary>
        private List<string> _Texts = null;

        /// <summary>
        /// 現在の行番号を取得します。
        /// </summary>
        public int CurrentRowNumber
        {
            get { return _CurrentIndex; }
        }

        /// <summary>
        /// 現在の行の入れ子の深さを示す値を取得します。
        /// </summary>
        public int Depth
        {
            get { return 0; } //never table nested.
        }

        /// <summary>
        /// 現在の行の列数を取得します。
        /// </summary>
        public int FieldCount
        {
            get { return _ColumnCount; }
        }

        /// <summary>
        /// データ リーダーが閉じているかどうかを示す値を取得します。
        /// </summary>
        public bool IsClosed
        {
            get { return _EOF; }
        }

        ///// <summary>
        ///// SQL ステートメントの実行によって変更、挿入、または削除された行の数を取得します。
        ///// </summary>
        public int RecordsAffected
        {
            get { return -1; }
        }

        /// <summary>
        /// 指定したインデックス位置にある列の値を取得します
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i]
        {
            get { return _Values[i]; }
        }
        /// <summary>
        /// 指定した名前の列の値を取得します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get { return this[ExcelOperator.ConvertColumnLiteralToNumber(name) - 1]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// IDataReader  オブジェクトを閉じます。
        /// </summary>
        public void Close()
        {
            _EOF = true;
            _Values.Clear();
            _Texts.Clear();
        }

        ///// <summary>
        ///// IDataReader  の列メタデータを記述する DataTable を返します。
        ///// </summary>
        ///// <returns></returns>
        public System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// バッチ SQL ステートメントの結果を読み込むときに、データ リーダーを次の結果に進めます。
        ///// </summary>
        ///// <returns></returns>
        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// IDataReader  を次のレコードに進めます。
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (this.IsClosed) return false;

            _Values.Clear();
            _Texts.Clear();

            var i = _CurrentIndex + 1;

            _EOF = (i > _LastIndex);

            if (_EOF)
            {
                _CurrentRow = null;
                return false;
            }

            //現在行オブジェクト取得
            var row = _XlOperator.GetRow(_Sheet, i, 1);
            _ComObjects.Push(row);
            _CurrentRow = row;

            //列ごとのセルデータ取得
            using (var r = new ComReleaser())
            {
                var fromidx = _ColumnOffset + 1;
                var toidx = _ColumnOffset + _ColumnCount;
                for (var c = fromidx; c <= toidx; c++)
                {
                    var cell = row.Columns[c];
                    r.Push(cell);
                    _Values.Add(cell.Value);
                    _Texts.Add(Convert.ToString(cell.Text));
                }
            }

            _CurrentIndex = i;

            return true;
        }

        /// <summary>
        /// 指定した列序数の IDataReader を返します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IDataReader GetData(int i)
        {
            return null;
        }

        /// <summary>
        /// 指定したフィールドが null に設定されているかどうかを示す値を返します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsDBNull(int i)
        {
            return _Values[i] == null;
        }

        /// <summary>
        /// 指定したフィールドのデータ型情報を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetDataTypeName(int i)
        {
            return this.GetFieldType(i).Name;
        }

        /// <summary>
        /// GetValue  から返される Object の型に対応する Type 情報を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Type GetFieldType(int i)
        {
            return _Values[i].GetType();
        }

        /// <summary>
        /// 指定した列の値をブール値として取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return Convert.ToBoolean(_Values[i]);
        }

        /// <summary>
        /// 指定した列の 8 ビット符号なし整数値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte GetByte(int i)
        {
            return Convert.ToByte(_Values[i]);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="i"></param>
        ///// <param name="fieldOffset"></param>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="length"></param>
        ///// <returns></returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定した列の文字値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public char GetChar(int i)
        {
            return Convert.ToChar(_Values[i]);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="i"></param>
        ///// <param name="fieldOffset"></param>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="length"></param>
        ///// <returns></returns>
        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定したフィールドの日時のデータ値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public DateTime GetDateTime(int i)
        {
            return Convert.ToDateTime(_Values[i]);
        }

        /// <summary>
        /// 指定したフィールドの固定位置数値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(_Values[i]);
        }

        /// <summary>
        /// 指定したフィールドの倍精度浮動小数点数を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double GetDouble(int i)
        {
            return Convert.ToDouble(_Values[i]);
        }

        /// <summary>
        /// 指定したフィールドの単精度浮動小数点数を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float GetFloat(int i)
        {
            return Convert.ToSingle(_Values[i]);
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定したフィールドの 16 ビット符号付き整数値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public short GetInt16(int i)
        {
            return Convert.ToInt16(_Values[i]);
        }

        /// <summary>
        /// 指定したフィールドの 32 ビット符号付き整数値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int GetInt32(int i)
        {
            return Convert.ToInt32(_Values[i]);
        }

        /// <summary>
        /// 指定したフィールドの 64 ビット符号付き整数値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public long GetInt64(int i)
        {
            return Convert.ToInt64(_Values[i]);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="i"></param>
        ///// <returns></returns>
        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        public int GetOrdinal(string name)
        {
            return ExcelOperator.ConvertColumnLiteralToNumber(name);
        }

        /// <summary>
        /// 指定したフィールドの文字列値を取得します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetString(int i)
        {
            return _Texts[i];
        }

        /// <summary>
        /// 指定したフィールドの値を返します。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object GetValue(int i)
        {
            return _Values[i];
        }

        /// <summary>
        /// オブジェクトの配列に現在のレコードの列値を設定します。
        /// </summary>
        /// <param name="values"></param>
        /// <returns>配列の Object のインスタンス数。</returns>
        public int GetValues(object[] values)
        {
            _Values.CopyTo(values);
            return values.Length;
        }

        /// <summary>
        /// 現在行セルの、Valueプロパティのリストを新しい配列にコピーします。
        /// </summary>
        /// <returns></returns>
        public object[] CopyToArray()
        {
            return _Values.ToArray();
        }

        /// <summary>
        /// 現在行セルの、Textプロパティのリストを新しい配列にコピーします。
        /// </summary>
        /// <returns></returns>
        public string[] CopyToTextArray()
        {
            return _Texts.ToArray();
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Excel行リーダーのインスタンスを生成します。
        /// </summary>
        /// <param name="xlOpe">ExcelOperator</param>
        /// <param name="sheetname">シート名</param>
        /// <param name="columnCount">リーダー列数</param>
        /// <param name="rowOffset">読み込み開始までスキップする行数</param>
        /// <param name="columnOffset">読み込み開始までスキップする列数</param>
        public ExcelRowReader(ExcelOperator xlOpe, string sheetname, int columnCount, int rowOffset, int columnOffset)
        {
            _XlOperator = xlOpe;
            _ColumnCount = columnCount;
            _ColumnOffset = columnOffset;

            _Values = new List<object>(columnCount);
            _Texts = new List<string>(columnCount);

            //シートオブジェクトの取得
            _Sheet = xlOpe.GetSheet(sheetname);
            _ComObjects.Push(_Sheet);

            //最終行インデックスの取得
            _LastIndex = xlOpe.GetLastRownum(sheetname);

            //読み込みポジションのリセット
            _CurrentIndex = rowOffset;
            _CurrentRow = null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージ状態を破棄します (マネージ オブジェクト)。
                    _Values.Clear();
                    _Texts.Clear();
                }

                // アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                _ComObjects.Dispose();
                // 大きなフィールドを null に設定します。
                _Values = null;
                _Texts = null;
                _CurrentRow = null;
                _Sheet = null;
                _ComObjects = null;
                _XlOperator = null;

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ExcelRowReader()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }//end class
}//end namespace
