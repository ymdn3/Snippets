using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
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
        #endregion
    }
}
