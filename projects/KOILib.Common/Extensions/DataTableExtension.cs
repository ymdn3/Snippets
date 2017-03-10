using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Extensions
{
    public static class DataTableExtension
    {
        #region System.Data.DataTable

        /// <summary>
        /// DataTableの各RowをExpandoObjectに変換します。
        /// http://neue.cc/2013/06/30_411.html
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> AsDynamic(this DataTable self)
        {
            return self.AsEnumerable().Select(x =>
            {
                IDictionary<string, object> dict = new ExpandoObject();
                foreach (DataColumn column in x.Table.Columns)
                {
                    var value = x[column];
                    if (value is System.DBNull) value = null;
                    dict.Add(column.ColumnName, value);
                }
                return (dynamic)dict;
            });
        }
        #endregion

    }
}
