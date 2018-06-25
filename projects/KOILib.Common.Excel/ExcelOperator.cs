using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// Microsoft Excel シートデータ操作クラス
    /// </summary>
    public class ExcelOperator
        : IDisposable
    {
        #region Static Members
        /// <summary>
        /// デフォルトシート名「Sheet1」
        /// </summary>
        public const string DefaultSheetName = "Sheet1";

        /// <summary>
        /// 列をあらわす文字を、列番号に変換します
        /// </summary>
        /// <param name="literal">列文字(A...IV...XFD)</param>
        /// <returns>列番号(1...256...16384)</returns>
        public static int ConvertColumnLiteralToNumber(string literal)
        {
            var @base = Char.ConvertToUtf32("Z", 0) - Char.ConvertToUtf32("A", 0) + 1;
            var colnum = 0;
            var lLength = literal.Length;

            for (var i = 1; i <= lLength; i++)
            {
                colnum += (Char.ConvertToUtf32(literal.ToUpper(), i - 1) - 0x40) * Convert.ToInt32(@base ^ (lLength - i));
            }
            return colnum;
        }
        #endregion

        #region Fields

        /// <summary>
        /// このクラスで使用する Microsoft Excel オブジェクト
        /// </summary>
        public ExcelObject ExcelObject { get; }

        /// <summary>
        /// このクラスで使用するブック
        /// </summary>
        public Workbook Workbook { get; }

        /// <summary>
        /// 解放が必要なCOMオブジェクトコレクション
        /// </summary>
        private ComReleaser _ComObjects = new ComReleaser();
        #endregion

        #region Methods (戻り値がCOMオブジェクトの関数は公開しない！)

        #region シートデータ取得
        /// <summary>
        /// 列数固定の行リーダーを作成します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="columnCount">列数</param>
        /// <param name="rowOffset">読み込み開始までスキップする行数</param>
        /// <param name="columnOffset">読み込み開始までスキップする列数</param>
        /// <returns></returns>
        public ExcelRowReader CreateRowReader(string sheetname, int columnCount, int rowOffset, int columnOffset)
        {
            return new ExcelRowReader(this, sheetname, columnCount, rowOffset, columnOffset);
        }

        /// <summary>
        /// 指定のシートに定義された「名前」のセル位置情報をHashSetで取得します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <returns></returns>
        public HashSet<ExcelCell> GetNamedRangeCells(string sheetname, string namedRange)
        {
            var cellSet = new HashSet<ExcelCell>();

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var ranges = GetNamedRanges(sheet, namedRange);
            foreach (Range range in ranges)
            {
                foreach (ExcelCell cell in ExcelCell.FromRange(range))
                    cellSet.Add(cell);

                _ComObjects.Push(range);
            }

            return cellSet;
        }

        /// <summary>
        /// 「ブック」に定義された「名前」のセル位置情報をHashSetで取得します
        /// </summary>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <returns></returns>
        public HashSet<ExcelCell> GetNamedRangeCells(string namedRange)
        {
            var cellSet = new HashSet<ExcelCell>();

            var ranges = GetNamedRanges(namedRange);
            foreach (Range range in ranges)
            {
                foreach (ExcelCell cell in ExcelCell.FromRange(range))
                    cellSet.Add(cell);

                _ComObjects.Push(range);
            }

            return cellSet;
        }

        /// <summary>
        /// 指定したシートの、指定した行列にあたるセルの値を取得します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        /// <param name="colnum">列番号</param>
        /// <returns></returns>
        public dynamic GetValueObject(string sheetname, int rownum, int colnum)
        {
            object v = null;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var cell = (Range)cells[RowIndex: rownum, ColumnIndex: colnum];
            _ComObjects.Push(cell);

            v = cell.Value;

            return v;
        }

        /// <summary>
        /// 指定した位置にあたるセルの値を取得します
        /// </summary>
        /// <param name="cell">ExcelCell</param>
        /// <returns></returns>
        public dynamic GetValueObject(ExcelCell cell)
        {
            return GetValueObject(cell.SheetName, cell.Row, cell.Column);
        }
        #endregion

        #region シート情報取得

        /// <summary>
        /// シート名取得
        /// </summary>
        /// <param name="sheetindex">シート見出しのもっとも左を1としたインデックス</param>
        /// <returns></returns>
        public string GetSheetName(int sheetindex)
        {
            var name = "";

            var sheet = GetSheet(sheetindex);
            _ComObjects.Push(sheet);

            name = sheet.Name;

            return name;
        }

        /// <summary>
        /// 指定のシートが存在するかを取得します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <returns></returns>
        public bool HasSheet(string sheetname)
        {
            var founds = false;

            var sheet = GetSheet(sheetname);
            if (sheet != null)
                _ComObjects.Push(sheet);

            founds = (sheet != null);

            return founds;
        }

        /// <summary>
        /// 指定したシートの、使われた範囲の最後のセル（LastCell）の行番号を取得します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <returns></returns>
        public int GetLastRownum(string sheetname)
        {
            var rownum = 0;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var lastcell = cells.SpecialCells(XlCellType.xlCellTypeLastCell);
            _ComObjects.Push(lastcell);

            rownum = lastcell.Row;

            return rownum;
        }

        /// <summary>
        /// 指定したシートの、使われた範囲の最後のセル（LastCell）の列番号を取得します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <returns></returns>
        public int GetLastColnum(string sheetname)
        {
            var colnum = 0;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var lastcell = cells.SpecialCells(XlCellType.xlCellTypeLastCell);
            _ComObjects.Push(lastcell);

            colnum = lastcell.Column;

            return colnum;
        }

        /// <summary>
        /// 指定のシートに定義された「名前」が存在するかを取得します。
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <returns></returns>
        public bool HasNamedRange(string sheetname, string namedRange)
        {
            var founds = false;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            //存在するかどうかのみを知るため、1個に限って取得
            var ranges = GetNamedRanges(sheet, namedRange, 1);
            foreach (var range in ranges)
                _ComObjects.Push(range);

            founds = (ranges.Count > 0);

            return founds;
        }

        /// <summary>
        /// ブックに定義された「名前」が存在するかを取得します
        /// </summary>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <returns></returns>
        public bool HasNamedRange(string namedRange)
        {
            var founds = false;

            //存在するかどうかのみを知るため、1個に限って取得
            var ranges = GetNamedRanges(namedRange, 1);
            foreach (var range in ranges)
                _ComObjects.Push(range);

            founds = (ranges.Count > 0);

            return founds;
        }

        /// <summary>
        /// 指定のシートに、指定の図形オブジェクトが存在するかを取得します。
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="shapename"></param>
        /// <returns></returns>
        public bool HasShape(string sheetname, string shapename)
        {
            var founds = false;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var shape = GetShape(sheet, shapename);
            if (shape != null)
                _ComObjects.Push(shape);

            founds = (shape != null);

            return founds;
        }
        #endregion

        #region セル値設定
        /// <summary>
        /// 指定シートのセルに、2次元表形式のデータを貼り付けます。
        /// 空値は空として貼り付けられるため、セルはクリアされます。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">貼り付ける2次元表形式データ</param>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">貼り付け開始セルの行番号</param>
        /// <param name="colnum">貼り付け開始セルの列番号</param>
        public void SetValue<T>(TableArray<T> value, string sheetname, int rownum, int colnum)
        {

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var from = (Range)cells[RowIndex: rownum, ColumnIndex: colnum];
            _ComObjects.Push(from);

            var to = from.Resize[RowSize: value.RowCount, ColumnSize: value.ColumnCount];
            _ComObjects.Push(to);

            to.Value = value.Array;
        }

        /// <summary>
        /// 指定したシートの単一セルに対して単一データを貼り付けます
        /// </summary>
        /// <param name="value">貼り付けるデータ</param>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        /// <param name="colnum">列番号</param>
        public void SetValue(object value, string sheetname, int rownum, int colnum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var from = (Range)cells[RowIndex: rownum, ColumnIndex: colnum];
            _ComObjects.Push(from);

            from.Value = value;
        }

        /// <summary>
        /// 指定したシートの「名前」で定義されたセルに対して単一データを貼り付けます。
        /// </summary>
        /// <param name="value">貼り付けるデータ</param>
        /// <param name="sheetname">シート名</param>
        /// <param name="namedRange">Excelでシート定義された名前</param>
        public void SetValue(object value, string sheetname, string namedRange)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Range[namedRange];
            _ComObjects.Push(cells);

            if (value is string)
                cells.NumberFormatLocal = "@";

            cells.Value = value;
        }
        #endregion

        #region 図形データ操作
        /// <summary>
        /// 指定したシートの、指定した図形オブジェクトのテキストを取得します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="shapename"></param>
        /// <returns></returns>
        [Obsolete("needs assembly [office, Version15]")]
        public string GetTextFromShape(string sheetname, string shapename)
        {
#if true
            throw new NotSupportedException("needs assembly [office, Version15].");
#else
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var shape = GetShape(sheet, shapename);
            if (shape == null)
                return null;

            _ComObjects.Push(shape);

            TextFrame2 textframe = shape.TextFrame2;
            _ComObjects.Push(textframe);

            TextRange2 textrange = textframe.TextRange;
            _ComObjects.Push(textrange);

            return textrange.Text;
#endif
        }

        /// <summary>
        /// 指定したシートの、指定した図形オブジェクトにテキストを設定します
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sheetname"></param>
        /// <param name="shapename"></param>
        [Obsolete("needs assembly [office, Version15]")]
        public void SetTextToShape(object value, string sheetname, string shapename)
        {
#if true
            throw new NotSupportedException("needs assembly [office, Version15].");
#else
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var shape = GetShape(sheet, shapename);
            if (shape == null)
                return;

            _ComObjects.Push(shape);

            TextFrame2 textframe = shape.TextFrame2;
            _ComObjects.Push(textframe);

            TextRange2 textrange = textframe.TextRange;
            _ComObjects.Push(textrange);

            textrange.Text = value;
#endif
        }
        #endregion

        #region 行列操作
        /// <summary>
        /// 指定行に行を挿入します。結果として、指定行は空行となります
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        /// <param name="count">挿入する行数</param>
        public void InsertRow(string sheetname, int rownum, int count)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var row = GetRow(sheet, rownum, count);
            _ComObjects.Push(row);

            row.Insert(XlInsertShiftDirection.xlShiftDown);
        }

        /// <summary>
        /// 指定列に列を挿入します。結果として、指定列は空列となります
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="colnum">列番号</param>
        /// <param name="count">挿入する列数</param>
        public void InsertColumn(string sheetname, int colnum, int count)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var col = GetColumn(sheet, colnum, count);
            _ComObjects.Push(col);

            col.Insert(XlInsertShiftDirection.xlShiftToRight);
        }

        /// <summary>
        /// 指定の1行をコピーし、指定行の次の行から挿入します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        /// <param name="count">コピー挿入する行数</param>
        public void CopyRow(string sheetname, int rownum, int count)
        {
            if (count <= 0) return;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var srcAddr = string.Format("{0}:{0}", rownum);
            var src = sheet.Range[srcAddr];
            _ComObjects.Push(src);

            src.Copy();

            var destAddr = string.Format("{0}:{1}", rownum + 1, rownum + count);
            var dest = sheet.Range[destAddr];
            _ComObjects.Push(dest);

            dest.Insert();
        }

        /// <summary>
        /// 指定した行から連続する複数行を、指定行へ挿入します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="fromRownum"></param>
        /// <param name="count"></param>
        /// <param name="toRownum"></param>
        public void CopyRow(string sheetname, int fromRownum, int count, int toRownum)
        {
            if (count <= 0) return;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var srcAddr = string.Format("{0}:{1}", fromRownum, fromRownum + count - 1);
            var src = sheet.Range[srcAddr];
            _ComObjects.Push(src);

            src.Copy();

            var destAddr = string.Format("{0}:{1}", toRownum, toRownum + count - 1);
            var dest = sheet.Range[destAddr];
            _ComObjects.Push(dest);

            dest.Insert(XlInsertShiftDirection.xlShiftDown);
        }

        /// <summary>
        /// 指定行より、指定数の行を削除します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        /// <param name="count">削除する行数</param>
        public void DeleteRow(string sheetname, int rownum, int count)
        {
            if (count <= 0) return;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var row = GetRow(sheet, rownum, count);
            _ComObjects.Push(row);

            row.Delete(XlDeleteShiftDirection.xlShiftUp);
        }

        /// <summary>
        /// 指定列より、指定数の列を削除します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="colnum">列番号</param>
        /// <param name="count">削除する列数</param>
        public void DeleteColumn(string sheetname, int colnum, int count)
        {
            if (count <= 0) return;

            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var col = GetColumn(sheet, colnum, count);
            _ComObjects.Push(col);

            col.Delete(XlDeleteShiftDirection.xlShiftToLeft);
        }

        /// <summary>
        /// 指定行の高さを自動調整します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="rownum"></param>
        public void AutoFitRow(string sheetname, int rownum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var row = GetRow(sheet, rownum, 1);
            _ComObjects.Push(row);

            row.AutoFit();
        }

        /// <summary>
        /// 指定列の幅を自動調整します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="colnum"></param>
        public void AutoFitColumn(string sheetname, int colnum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var col = GetColumn(sheet, colnum, 1);
            _ComObjects.Push(col);

            col.AutoFit();
        }
        #endregion

        #region セル操作
        /// <summary>
        /// 指定範囲のセルを結合します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="topRownum"></param>
        /// <param name="bottomRownum"></param>
        /// <param name="leftColnum"></param>
        /// <param name="rightColnum"></param>
        public void MergeCell(string sheetname, int topRownum, int bottomRownum, int leftColnum, int rightColnum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var cell1 = cells[RowIndex:topRownum, ColumnIndex:leftColnum];
            _ComObjects.Push(cell1);

            var cell2 = cells[RowIndex:bottomRownum, ColumnIndex:rightColnum];
            _ComObjects.Push(cell2);
                
            var range = cells.Range[Cell1:cell1, Cell2:cell2];
            _ComObjects.Push(range);

            range.Merge();
        }

        /// <summary>
        /// 指定範囲のセルの結合を解除します
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="topRownum"></param>
        /// <param name="bottomRownum"></param>
        /// <param name="leftColnum"></param>
        /// <param name="rightColnum"></param>
        public void UnMergeCell(string sheetname, int topRownum, int bottomRownum, int leftColnum, int rightColnum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            var cell1 = cells[RowIndex: topRownum, ColumnIndex: leftColnum];
            _ComObjects.Push(cell1);

            var cell2 = cells[RowIndex: bottomRownum, ColumnIndex: rightColnum];
            _ComObjects.Push(cell2);

            var range = cells.Range[Cell1: cell1, Cell2: cell2];
            _ComObjects.Push(range);

            range.UnMerge();
        }
        #endregion

        #region 改ページ指定
        /// <summary>
        /// 指定行の直前に改ページを設定します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="rownum">行番号</param>
        public void InsertHPageBreak(string sheetname, int rownum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var pgbrk = sheet.HPageBreaks;
            _ComObjects.Push(pgbrk);

            var row = GetRow(sheet, rownum, 1);
            _ComObjects.Push(row);

            pgbrk.Add(row);
        }

        /// <summary>
        /// 指定列の直前に改ページを設定します
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <param name="colnum">列番号</param>
        public void InsertVPageBreak(string sheetname, int colnum)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var pgbrk = sheet.VPageBreaks;
            _ComObjects.Push(pgbrk);

            var col = GetColumn(sheet, colnum, 1);
            _ComObjects.Push(col);

            pgbrk.Add(col);
        }
#endregion

#region シート操作
        /// <summary>
        /// シートのアクティブ化
        /// </summary>
        /// <param name="sheetname">対象のシート名</param>
        public void ActivateSheetTo(string sheetname)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            sheet.Activate();
        }
        
        /// <summary>
        /// シートのクリア
        /// </summary>
        /// <param name="sheetname">シート名</param>
        public void ClearSheet(string sheetname)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            var cells = sheet.Cells;
            _ComObjects.Push(cells);

            cells.Clear();
        }

        /// <summary>
        /// シートコピー
        /// </summary>
        /// <param name="srcSheetname">コピー元のシート名</param>
        /// <param name="newSheetname">コピー先のシート名</param>
        /// <param name="toAfter"><c>True</c>のとき、コピー元シートの右側、<c>False</c>のとき、コピー元シートの左側に新たなシートを作成します。</param>
        public void CopySheet(string srcSheetname, string newSheetname, bool toAfter)
        {
            var srcsheet = GetSheet(srcSheetname);
            _ComObjects.Push(srcsheet);

            if (toAfter)
                srcsheet.Copy(After: srcsheet);
            else
                srcsheet.Copy(Before: srcsheet);

            //Copy後、生成されたシートはアクティブシートとなっている
            var newsheet = (Worksheet)Workbook.ActiveSheet;
            _ComObjects.Push(newsheet);

            newsheet.Name = newSheetname;
        }

        /// <summary>
        /// シート移動
        /// </summary>
        /// <param name="srcSheetname">移動対象のシート名</param>
        /// <param name="baseSheetname">移動先基準のシート名</param>
        /// <param name="toAfter"><c>True</c>のとき、基準シートの右側、<c>False</c>のとき、基準シートの左側にシートを移動します。</param>
        public void MoveSheet(string srcSheetname, string baseSheetname, bool toAfter)
        {
            var srcsheet = GetSheet(srcSheetname);
            _ComObjects.Push(srcsheet);

            var basesheet = GetSheet(baseSheetname);
            _ComObjects.Push(basesheet);

            if (toAfter)
                srcsheet.Move(After: basesheet);
            else
                srcsheet.Move(Before: basesheet);
        }

        /// <summary>
        /// シート削除
        /// </summary>
        /// <param name="sheetname">シート名</param>
        public void DeleteSheet(string sheetname)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            sheet.Delete();
        }
#endregion

#region ブック保存
        /// <summary>
        /// ブック保存
        /// </summary>
        /// <param name="filepath">保存先パス</param>
        public void SaveAs(string filepath)
        {
            //ファイルが存在する場合にExcelでSaveAsすると、上書き確認ダイアログが表示されてしまうため、あらかじめ削除する。
            System.IO.File.Delete(filepath);
            Workbook.SaveAs(filepath);
        }

        /// <summary>
        /// ブック保存
        /// </summary>
        public void Save()
        {
            Workbook.Save();
        }
        #endregion

        #region ブック出力
        /// <summary>
        /// ブックをPDF形式で出力します
        /// </summary>
        /// <param name="filepath">出力先パス。拡張子は「.pdf」に変更します。</param>
        public void ExportPDF(string filepath)
        {
            var sheets = Workbook.Worksheets;
            _ComObjects.Push(sheets);

            //複数シートがある場合は、全て選択する
            if (sheets.Count > 1)
                sheets.Select();

            //拡張子が".pdf"でない場合に".pdf"が付加されてしまうため、あらかじめ拡張子を変更する
            var file = System.IO.Path.ChangeExtension(filepath, ".pdf");
            System.IO.File.Delete(file);

            var type = XlFixedFormatType.xlTypePDF;
            var quarity = XlFixedFormatQuality.xlQualityStandard;
            Workbook.ExportAsFixedFormat(Type: type, Filename: filepath, Quality: quarity, IncludeDocProperties: true, IgnorePrintAreas: false, OpenAfterPublish: false);
        }
        #endregion

        #region ブック・シート印刷
        /// <summary>
        /// ワークシート印刷
        /// </summary>
        /// <param name="sheetname">出力するシート名</param>
        /// <param name="pageFrom">出力する開始ページ。省略時は、<c>1</c></param>
        /// <param name="pageTo">出力する終了ページ。省略時は、<c>0</c>で、ページの指定をしません。</param>
        /// <param name="copies">コピー部数。省略時は、<c>1</c></param>
        /// <param name="printerName">プリンター名（System.Drawing.Printing.PrinterSettings.InstalledPrinters から取得できるものを設定することを推奨）。省略可能。</param>
        public void PrintOut(string sheetname, int pageFrom = 1, int pageTo = 0, int copies = 1, string printerName = null)
        {
            var sheet = GetSheet(sheetname);
            _ComObjects.Push(sheet);

            if (string.IsNullOrEmpty(printerName))
            {
                if (pageTo > 0)
                    sheet.PrintOutEx(From: pageFrom, To: pageTo, Copies: copies);
                else
                    sheet.PrintOutEx(From: pageFrom, Copies: copies);
            }
            else
            {
                if (pageTo > 0)
                    sheet.PrintOutEx(From: pageFrom, To: pageTo, Copies: copies, ActivePrinter: printerName);
                else
                    sheet.PrintOutEx(From: pageFrom, Copies: copies, ActivePrinter: printerName);
            }
        }

        /// <summary>
        /// ブック印刷
        /// </summary>
        /// <param name="pageFrom">出力する開始ページ。省略時は、<c>1</c></param>
        /// <param name="pageTo">出力する終了ページ。省略時は、<c>0</c>で、ページの指定をしません。</param>
        /// <param name="copies">コピー部数。省略時は、<c>1</c></param>
        /// <param name="printerName">プリンター名（System.Drawing.Printing.PrinterSettings.InstalledPrinters から取得できるものを設定することを推奨）。省略可能。</param>
        public void PrintOut(int pageFrom = 1, int pageTo = 0, int copies = 1, string printerName = null)
        {
            if (string.IsNullOrEmpty(printerName))
            {
                if (pageTo > 0)
                    Workbook.PrintOutEx(From: pageFrom, To: pageTo, Copies: copies);
                else
                    Workbook.PrintOutEx(From: pageFrom, Copies: copies);
            }
            else
            {
                if (pageTo > 0)
                    Workbook.PrintOutEx(From: pageFrom, To: pageTo, Copies: copies, ActivePrinter: printerName);
                else
                    Workbook.PrintOutEx(From: pageFrom, Copies: copies, ActivePrinter: printerName);
            }
        }
#endregion

#region 非公開
        /// <summary>
        /// 指定の名前のシートオブジェクトを取得します。
        /// </summary>
        /// <param name="sheetname">シート名</param>
        /// <returns></returns>
        protected internal Worksheet GetSheet(string sheetname)
        {
            var sheets = Workbook.Worksheets;
            _ComObjects.Push(sheets);

            foreach (Worksheet s in sheets)
            {
                if (s.Name == sheetname)
                    return s;

                _ComObjects.Push(s);
            }

            return null;
        }

        /// <summary>
        /// 指定のインデックスのシートオブジェクトを取得します。
        /// </summary>
        /// <param name="sheetindex">シート見出しのもっとも左を1としたインデックス</param>
        /// <returns></returns>
        protected internal Worksheet GetSheet(int sheetindex)
        {
            Worksheet sheet = null;

            var sheets = Workbook.Worksheets;
            _ComObjects.Push(sheets);

            if (sheetindex <= 0 || sheets.Count < sheetindex)
            {
                //out of range.
                sheet = Workbook.ActiveSheet;
            }
            else
            {
                sheet = sheets.Item[sheetindex];
            }
            return sheet;
        }

        /// <summary>
        /// 指定の行番号の行Rangeを取得します
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rownum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected internal Range GetRow(Worksheet sheet, int rownum, int count)
        {
            Range range = null;

            var rows = sheet.Rows;
            _ComObjects.Push(rows);

            var startRow = (Range)rows[rownum];
            _ComObjects.Push(startRow);

            range = startRow.Resize[RowSize: count];

            return range;
        }

        /// <summary>
        /// 指定の列番号の列Rangeを取得します
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="colnum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected internal Range GetColumn(Worksheet sheet, int colnum, int count)
        {
            Range range = null;

            var cols = sheet.Columns;
            _ComObjects.Push(cols);

            var startCol = (Range)cols[colnum];
            _ComObjects.Push(startCol);

            range = startCol.Resize[ColumnSize: count];

            return range;
        }

        /// <summary>
        /// 指定のシートに定義された「名前」のRangeオブジェクトのHashSetを取得します
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <param name="limitcount">取得する要素数。指定しないとき無制限</param>
        /// <returns></returns>
        protected internal HashSet<Range> GetNamedRanges(Worksheet sheet, string namedRange, int limitcount = -1)
        {
            var ranges = new HashSet<Range>();
            if (limitcount == 0) return ranges;

            var names = sheet.Names;
            _ComObjects.Push(names);

            foreach (Name n in names)
            {
                _ComObjects.Push(n);
                if (n.Name == namedRange)
                {
                    ranges.Add(n.RefersToRange);
                    if (ranges.Count == limitcount) break;
                }
            }

            return ranges;
        }

        /// <summary>
        /// 「ブック」に定義された「名前」のRangeオブジェクトのHashSetを取得します
        /// </summary>
        /// <param name="namedRange">Excel定義の「名前」</param>
        /// <param name="limitcount">取得する要素数。指定しないとき無制限</param>
        /// <returns></returns>
        protected internal HashSet<Range> GetNamedRanges(string namedRange, int limitcount = -1)
        {
            var ranges = new HashSet<Range>();
            if (limitcount == 0) return ranges;

            var names = Workbook.Names;
            _ComObjects.Push(names);

            foreach (Name n in names)
            {
                _ComObjects.Push(n);
                if (n.Name == namedRange)
                {
                    ranges.Add(n.RefersToRange);
                    if (ranges.Count == limitcount) break;
                }
            }
        
            return ranges;
        }

        /// <summary>
        /// 指定の名前の図形オブジェクトを取得します
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="shapename"></param>
        /// <returns></returns>
        protected internal Shape GetShape(Worksheet sheet, string shapename)
        {
            var shapes = sheet.Shapes;
            _ComObjects.Push(shapes);

            foreach (Shape shape in shapes)
            {
                if (shape.Name == shapename)
                    return shape;

                _ComObjects.Push(shape);
            }

            return null;
        }
#endregion

#endregion

#region Constructors
        /// <summary>
        /// Excelファイルを指定して、このクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="xlobj">使用する Microsoft Excel アプリケーションオブジェクト</param>
        /// <param name="filepath">オープンする Microsoft Excel ファイル</param>
        public ExcelOperator(ExcelObject xlobj, string filepath)
        {
            ExcelObject = xlobj;

            var books = xlobj.Instance.Workbooks;
            _ComObjects.Push(books);

            Workbook = books.Open(Filename: filepath);
            _ComObjects.Push(Workbook);

            //このブックの初期状態を保存済み扱いとする
            Workbook.Saved = true;
        }

        /// <summary>
        /// このクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="xlobj">使用する Microsoft Excel アプリケーションオブジェクト</param>
        public ExcelOperator(ExcelObject xlobj)
        {
            ExcelObject = xlobj;

            //シート数1のブックを生成
            var books = xlobj.Instance.Workbooks;
            _ComObjects.Push(books);

            Workbook = books.Add(XlWBATemplate.xlWBATWorksheet);
            _ComObjects.Push(Workbook);

            var sheets = Workbook.Worksheets;
            _ComObjects.Push(sheets);

            using (var r = new ComReleaser())
            {
                var sheet = (Worksheet)sheets[1];
                r.Push(sheet);
                sheet.Activate();
            }

            //このブックの初期状態を保存済み扱いとする
            Workbook.Saved = true;
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
                }

                // アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。

                //ダイアログなし終了のためにブックの初期状態を保存済み扱いとする
                if (Workbook != null)
                    Workbook.Saved = true;

                // 大きなフィールドを null に設定します。
                _ComObjects.Dispose();
                _ComObjects = null;

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ExcelOperator()
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
