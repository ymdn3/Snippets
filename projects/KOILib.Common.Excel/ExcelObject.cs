using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// Microsoft Excel アプリケーションオブジェクトクラス
    /// </summary>
    public class ExcelObject
        : IDisposable
    {
        #region Static Members
        /// <summary>
        /// ファイル拡張子
        /// </summary>
        public const string Extension = ".xlsx";
        
        /// <summary>
        /// ファイル拡張子(.xls)
        /// </summary>
        public const string ExtensionOld = ".xls";

        /// <summary>
        /// OpenFileDialog等向けフィルタ文字列
        /// </summary>
        public static readonly string ExtFilter = String.Format("Excel ブック (*{0})|*{0}|Excel 97-2003 ブック (*{1})|*{1}", Extension, ExtensionOld);

        /// <summary>
        /// OpenFileDialog等向けフィルタ文字列(xlsのみ)
        /// </summary>
        public static readonly string ExtFilterOld = String.Format("Excel 97-2003 ブック (*{0})|*{0}", ExtensionOld);
        #endregion

        #region Fields
        /// <summary>
        /// エクセルアプリケーションオブジェクトを取得します。
        /// </summary>
        public Application Instance { get; private set; }

        /// <summary>
        /// クラスインスタンスの破棄時にエクセルアプリケーションをともに終了するかどうかを取得または設定します。
        /// </summary>
        public bool QuitOnDisposing { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ExcelObject()
        {
            Instance = new Application();
            Instance.Visible = false;
            Instance.DisplayAlerts = false;

            QuitOnDisposing = true;
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
                if (Instance != null)
                {
                    if (QuitOnDisposing) Instance.Quit();

                    ComReleaser.ReleaseComObject(Instance);
                }
                // 大きなフィールドを null に設定します。
                Instance = null;

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ExcelObject()
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
