using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Excel
{
    /// <summary>
    /// COM オブジェクト解放用スタッククラス
    /// </summary>
    public class ComReleaser
        : Stack<object>, IDisposable
    {
        #region Static Members
        /// <summary>
        /// COM オブジェクトへの参照を解放します
        /// </summary>
        /// <param name="o"></param>
        public static void ReleaseComObject(object o)
        {
            while (Marshal.ReleaseComObject(o) > 0);
        }
        #endregion

        /// <summary>
        /// スタックしている先頭のオブジェクト参照のみを解放します
        /// </summary>
        public void ReleasePeekOne()
        {
            if (this.Count > 0) ReleaseComObject(this.Pop());
        }

        /// <summary>
        /// スタックしているすべてのオブジェクト参照を解放します
        /// </summary>
        public void ReleaseAll()
        {
            while (this.Count > 0) this.ReleasePeekOne();
        }

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
                try { this.ReleaseAll(); }
                catch { }

                // 大きなフィールドを null に設定します。
                this.Clear();

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ComReleaser()
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
