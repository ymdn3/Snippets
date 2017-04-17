using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class TypeExtension
    {
        #region System.Type
        /// <summary>
        /// https://msdn.microsoft.com/ja-jp/library/ms366789.aspx
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type self)
        {
            return (self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// リフレクションを介して、public staticなメソッドをInvokeします。
        /// optional引数をデフォルトで使用する場合は Type.Missing を使う必要があります。
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="self"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TReturn InvokeMethod<TReturn>(this Type self, string methodName, object[] methodArgs = null)
        {
            const BindingFlags attr = BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod;
            if (self == default(Type))
                return (TReturn)self.InvokeMember(methodName, attr, null, null, methodArgs); //→System.MissingMethodException

            var method = self.GetMethods(attr)
                .FirstOrDefault(x => x.Name == methodName);
            if (method != null)
                return (TReturn)method.Invoke(null, attr, null, methodArgs, System.Globalization.CultureInfo.CurrentCulture);
            else
                return self.GetTypeInfo().BaseType.InvokeMethod<TReturn>(methodName, methodArgs);
        }
        #endregion
    }
}
