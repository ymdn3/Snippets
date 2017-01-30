using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// Microsoft Excel の Range 型の単一のセルをあらわします。
    /// COM オブジェクト自体を公開することを制限するためのもので、基本的に参照のみが可能です。
    /// セル自体の情報がほしい場合は、このクラスを拡張していってください。
    /// </summary>
    public class ExcelCell
    {
        #region Static Members
        /// <summary>
        /// 指定のRangeからこのクラスのインスタンスを生成し、HashSetとして返します。
        /// </summary>
        /// <param name="range">Range</param>
        /// <returns></returns>
        public static HashSet<ExcelCell> FromRange(Range range)
        {
            var addrSet = new HashSet<ExcelCell>();
            using (var r = new ComReleaser())
            {
                var cells = range.Cells;
                r.Push(cells);
                foreach (Range c in cells)
                {
                    r.Push(c);
                    var sheet = c.Worksheet;
                    r.Push(sheet);

                    var cell = new ExcelCell(sheet.Name, c.Row, c.Column)
                    {
                        Value = c.Value,
                        Formula = c.Formula,
                        Text = Convert.ToString(c.Text),
                        AllowEdit = c.AllowEdit,
                        Locked = c.Locked,
                        Address = c.Address,
                        AddressLocal = c.AddressLocal,
                        NumberFormat = c.NumberFormat,
                        NumberFormatLocal = c.NumberFormatLocal,
                        RowHeight = c.RowHeight,
                        ColumnWidth = c.ColumnWidth,
                    };
                    addrSet.Add(cell);
                }
            }
            return addrSet;
        }
        #endregion

        #region Fields
        public string SheetName { get; }
        public int Row { get; }
        public int Column { get; }
        public object Value { get; private set; }
        public object Formula { get; private set; }
        public string Text { get; private set; }
        public bool AllowEdit { get; private set; }
        public object Locked { get; private set; }
        public string Address { get; private set; }
        public string AddressLocal { get; private set; }
        public object NumberFormat { get; private set; }
        public object NumberFormatLocal { get; private set; }
        public object RowHeight { get; private set; }
        public object ColumnWidth { get; private set; }
        #endregion

        #region Constructors
        public ExcelCell(string sheetname, int rownum, int colnum)
            : this(sheetname, rownum, colnum, null)
        {
        }
        public ExcelCell(string sheetname, int rownum, int colnum, object value)
        {
            this.SheetName = sheetname;
            this.Row = rownum;
            this.Column = colnum;
            this.Value = value;

            this.Formula = null;
            this.Text = "";
            this.AllowEdit = false;
            this.Locked = null;
            this.Address = null;
            this.AddressLocal = null;
            this.NumberFormat = null;
            this.NumberFormatLocal = null;
            this.RowHeight = 0;
            this.ColumnWidth = 0;
        }
        #endregion

    }//end class
}//end namespaece
