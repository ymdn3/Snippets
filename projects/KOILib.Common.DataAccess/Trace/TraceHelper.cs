using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common;

namespace KOILib.Common.DataAccess.Trace
{
    internal static class TraceHelper
    {
        internal static string ToLogString(this DbParameterCollection parameters)
        {
            var t = new StringList(parameters
                .Cast<DbParameter>()
                .Select(p => p.ToLogString())
            );
            return t.Decorate(sep: ",").ToString();
        }

        internal static string ToLogString(this DbParameter parameter)
        {
            return string.Format("'{0}':'{1}'", parameter.ParameterName, parameter.Value);
        }

    }
}
