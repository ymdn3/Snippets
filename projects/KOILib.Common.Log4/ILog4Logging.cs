using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Log4
{
    /// <summary>
    /// Loggerを経由してログ出力する機能I/F
    /// </summary>
    public interface ILog4Logging
    {
        LoggerBase Logger { get; }
        LogLevel LogLevel { get; set; }
    }
}
