using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    public static class FileInfoExtension
    {
        #region System.IO.FileInfo
        /// <summary>
        /// 指定したファイルが他のプロセスにより使用中であるかどうかを検証します。
        /// </summary>
        /// <param name="self">検証するファイル</param>
        /// <returns>他のプロセスにより使用中である場合、<c>True</c></returns>
        public static bool IsUsing(this System.IO.FileInfo self)
        {
            //ファイルが無い
            if (!self.Exists) return false;

            Stream fs = null;
            try
            {
                //共有不可でオープン
                using (fs = self.Open(FileMode.Open, FileAccess.Read)) { fs.Close(); }
                return false;
            }
            catch (Exception)
            {
                try { if (fs != null) fs.Dispose(); }
                catch (Exception) { }
                return true;
            }
        }
        #endregion

    }
}
