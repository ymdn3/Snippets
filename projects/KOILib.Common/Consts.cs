using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    /// <summary>
    /// 定数クラス。
    /// アクセスレベルが internal 以上の定数には、const キーワードを使わないこと
    /// （定数値はビルド時に参照元へ埋め込まれるため、値が変化した際、参照元もリビルドが必要になってしまうのを回避する）
    /// </summary>
    public static class Consts
    {

        //64bitプロセスでもGetHashCode()が返す長さは変わらないので。
        public static readonly int INSTANCE_HASHCODE_LEN = 32 / 4;//(System.Environment.Is64BitProcess ? 64 : 32) / 4;

        public static readonly Encoding SJIS_ENCODING = Encoding.GetEncoding("Shift_JIS");
    }
}
