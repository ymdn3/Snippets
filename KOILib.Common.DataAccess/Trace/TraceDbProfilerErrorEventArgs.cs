using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Data;

namespace KOILib.Common.DataAccess.Trace
{
    internal class TraceDbProfilerErrorEventArgs
        : TraceDbProfilerEventArgs
    {
        public Exception Exception { get; private set; }

        public TraceDbProfilerErrorEventArgs(Stopwatch stopwatch, DateTime date, IDbCommand command, SqlExecuteType executeType, Exception exception)
            : base(stopwatch, date, command, executeType)
        {
            Exception = exception;
        }
    }
}
