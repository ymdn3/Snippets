using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core
{
    /// <summary>
    /// 列挙体クラスのstaticメソッドのうち、enumtype を型パラメータで代理定義したクラス
    /// </summary>
    /// <typeparam name="TEnum">列挙体</typeparam>
    public static class EnumEx<TEnum>
        where TEnum : struct
    {
        /// <summary>
        /// 指定した列挙体に含まれている定数の名前の配列を取得します。
        /// </summary>
        /// <returns>TEnum に含まれている定数の名前の文字列配列。</returns>
        public static string[] GetNames()
        {
            ChecksTEnum();
            return Enum.GetNames(typeof(TEnum));
        }
        /// <summary>
        /// 指定した列挙体の基になる型を返します。
        /// </summary>
        /// <returns>TEnum の基になる型。</returns>
        public static Type GetUnderlyingType()
        {
            ChecksTEnum();
            return Enum.GetUnderlyingType(typeof(TEnum));
        }
        /// <summary>
        /// 指定した列挙体に含まれている定数の値の配列を取得します。
        /// </summary>
        /// <returns>TEnum に含まれている定数の値を格納する配列。</returns>
        public static Array GetValues()
        {
            ChecksTEnum();
            return Enum.GetValues(typeof(TEnum));
        }
        /// <summary>
        /// 指定した値を持つ定数が指定した列挙体に存在するかどうかを示す値を返します。
        /// </summary>
        /// <param name="value">TEnum 内の定数の値または名前。</param>
        /// <returns>TEnum 内の定数の値が value と等しい場合は true。それ以外の場合は false。</returns>
        public static bool IsDefined(object value)
        {
            ChecksTEnum();
            return Enum.IsDefined(typeof(TEnum), value);
        }
        /// <summary>
        /// 文字列形式での 1 つ以上の列挙定数の名前または数値を、等価の列挙オブジェクトに変換します。
        /// </summary>
        /// <param name="value">変換する名前または値が含まれている文字列。</param>
        /// <returns>値が value により表される TEnum 型のオブジェクト。</returns>
        public static TEnum Parse(string value)
        {
            ChecksTEnum();
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }
        /// <summary>
        /// 文字列形式での 1 つ以上の列挙定数の名前または数値を、等価の列挙オブジェクトに変換します。
        /// 演算で大文字と小文字を区別しないかどうかをパラメーターで指定します。
        /// </summary>
        /// <param name="value">変換する名前または値が含まれている文字列。</param>
        /// <param name="ignoreCase">大文字と小文字を区別しない場合は true。大文字と小文字を区別する場合は false。</param>
        /// <returns>値が value により表される TEnum 型のオブジェクト。</returns>
        public static TEnum Parse(string value, bool ignoreCase)
        {
            ChecksTEnum();
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }
        /// <summary>
        /// 文字列形式での 1 つ以上の列挙定数の名前または数値を、等価の列挙オブジェクトに変換します。
        /// 戻り値は、変換が成功したかどうかを示します。
        /// </summary>
        /// <param name="value">変換する列挙定数の名前または基になる値の文字列形式。</param>
        /// <param name="result">
        /// このメソッドから制御が戻るときに、result には、解析操作が成功したときに値が value で表される TEnum 型のオブジェクトが格納されます。
        /// 解析操作が失敗した場合、result には TEnum の基になる型の既定値が格納されます。
        /// この値が TEnum 列挙型のメンバーである必要がないことに注意してください。
        /// このパラメーターは初期化せずに渡されます。
        /// </param>
        /// <returns>value パラメーターが正常に変換された場合は true。それ以外の場合は false。</returns>
        public static bool TryParse(string value, out TEnum result)
        {
            ChecksTEnum();
            return Enum.TryParse(value, out result);
        }
        /// <summary>
        /// 文字列形式での 1 つ以上の列挙定数の名前または数値を、等価の列挙オブジェクトに変換します。 
        /// 演算で大文字と小文字を区別するかどうかをパラメーターで指定します。 戻り値は、変換が成功したかどうかを示します。
        /// </summary>
        /// <param name="value">変換する列挙定数の名前または基になる値の文字列形式。</param>
        /// <param name="ignoreCase">大文字と小文字を区別しない場合は true。大文字と小文字を区別する場合は false。</param>
        /// <param name="result">
        /// このメソッドから制御が戻るときに、result には、解析操作が成功したときに値が value で表される TEnum 型のオブジェクトが格納されます。
        /// 解析操作が失敗した場合、result には TEnum の基になる型の既定値が格納されます。
        /// この値が TEnum 列挙型のメンバーである必要がないことに注意してください。
        /// このパラメーターは初期化せずに渡されます。 
        /// </param>
        /// <returns>value パラメーターが正常に変換された場合は true。それ以外の場合は false。</returns>
        public static bool TryParse(string value, bool ignoreCase, out TEnum result)
        {
            ChecksTEnum();
            return Enum.TryParse(value, ignoreCase, out result);
        }
        /// <summary>
        /// 型パラメータがEnum型であるかをチェックします。
        /// ジェネリックの型パラメータ制約には、Enumそのものを指定することができないため、プログラムで制約の保証を実装します。
        /// </summary>
        private static void ChecksTEnum()
        {
            var t = typeof(TEnum);
            if (!t.IsEnum) throw new ArgumentException("本クラスの型パラメータは列挙値型である必要があります。：" + t.Name);
        }

    }//end class
}//end namespace
