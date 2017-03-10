using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FieldAttributeBase : Attribute
    {
        public static TAttrib GetAttribute<TEnum, TAttrib>(object e)
            where TEnum : struct
            where TAttrib : Attribute
        {
            var type = typeof(TEnum);
            var field = type.GetField(Enum.GetName(type, e));
            if (field == null)
                return default(TAttrib);
            return field.GetCustomAttributes(typeof(TAttrib), false).Cast<TAttrib>().FirstOrDefault();
        }

        public static TAttrib GetAttribute<TClass, TAttrib>(string fieldname)
            where TClass : class
            where TAttrib : Attribute
        {
            var type = typeof(TClass);
            var fields = type.GetMember(fieldname);
            if (fields == null || fields.Length == 0)
                return default(TAttrib);
            var field = fields[0];
            return field.GetCustomAttributes(typeof(TAttrib), false).Cast<TAttrib>().FirstOrDefault();
        } 
    }
}
