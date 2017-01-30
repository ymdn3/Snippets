using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Core.Extensions
{
    public static class DateTimeExtension
    {
        #region System.DateTime
        /// <summary>
        /// 年度を取得します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="monthNendoFrom">年度開始月[1-12]</param>
        /// <returns></returns>
        public static string ToNendo(this System.DateTime self, int monthNendoFrom)
        {
            //参考: RETURN(TO_CHAR(TRUNC(AD_MONTHS(IN_DATE, -(IN_FIRST_MONTH-1)), 'YEAR'), 'YYYY'));
            return self.AddMonths(-(monthNendoFrom - 1)).ToString("yyyy");
        }
        #endregion

    }
}
