using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// 2次元表形式の配列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TableArray<T>
    {
        #region Static Members
        /// <summary>
        /// 行要素インデックス
        /// </summary>
        private const int ROWDIM = 0;

        /// <summary>
        /// 列要素インデックス
        /// </summary>
        private const int COLDIM = 1;

        /// <summary>
        /// DataTableよりインスタンスを生成します
        /// </summary>
        /// <param name="dt">参照元DataTable</param>
        /// <param name="withHeader">1行目に列ヘッダーを出力する場合、<c>True</c></param>
        /// <returns>配列データ</returns>
        public static TableArray<object> FromDataTable(DataTable dt, bool withHeader)
        {
            //列名を出力する場合は1行ずらす
            var headerRowOffset = withHeader ? 1 : 0;

            var table = new TableArray<object>(dt.Rows.Count + headerRowOffset, dt.Columns.Count);

            //列名出力の有無
            if (withHeader)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    table[0, i] = dt.Columns[i].ColumnName;
                }
            }
            //データ行の収集
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var dr = dt.Rows[i];
                for (var j = 0; j < dr.ItemArray.Length; j++)
                {
                    table[i + headerRowOffset, j] = dr.ItemArray[j];
                }
            }
            return table;
        }

        /// <summary>
        /// 配列から1行(横方向)のインスタンスを生成します。
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        public static TableArray<T> FromArrayAsRow(params T[] argv)
        {
            var table = new TableArray<T>(1, argv.Length);
            for (var i = 0; i < argv.Length; i++)
            {
                table[0, i] = argv[i];
            }
            return table;
        }

        /// <summary>
        /// 配列から1列(縦方向)のインスタンスを生成します。
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        public static TableArray<T> FromArrayAsColumn(params T[] argv)
        {
            var table = new TableArray<T>(argv.Length, 1);
            for (var i = 0; i < argv.Length; i++)
            {
                table[i, 0] = argv[i];
            }
            return table;
        }
        #endregion

        /// <summary>
        /// 配列インスタンス
        /// </summary>
        public Array Array { get; }

        /// <summary>
        /// 指定位置要素へのアクセサ
        /// </summary>
        /// <param name="rowidx">行インデックス</param>
        /// <param name="columnidx">列インデックス</param>
        /// <returns></returns>
        public T this[int rowidx, int columnidx]
        {
            get
            {
                return (T)Array.GetValue(rowidx, columnidx);
            }
            set
            {
                Array.SetValue(value, rowidx, columnidx);
            }
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get { return Array.GetLength(ROWDIM); }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount
        {
            get { return Array.GetLength(COLDIM); }
        }

        #region Constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rowcount">行数</param>
        /// <param name="columncount">列数</param>
        public TableArray(int rowcount, int columncount)
        {
            Array = System.Array.CreateInstance(typeof(T), rowcount, columncount);
        }
        #endregion

    }//end class
}//end namespace
