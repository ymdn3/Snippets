using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FieldAttributeBase : Attribute
    {
        public static TAttrib GetAttribute<T, TAttrib>(object e)
            where T : struct
            where TAttrib : FieldAttributeBase
        {
            var type = typeof(T);
            return type.GetField(Enum.GetName(type, e)).GetCustomAttributes(typeof(TAttrib), false).Cast<TAttrib>().FirstOrDefault();
        }
    }
}
