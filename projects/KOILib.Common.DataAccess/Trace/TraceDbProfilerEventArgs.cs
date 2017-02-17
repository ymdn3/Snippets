using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common.Core.Extensions;
using StackExchange.Profiling.Data;

namespace KOILib.Common.DataAccess.Trace
{
    internal class TraceDbProfilerEventArgs
        : EventArgs
    {
        public DateTime Date { get; private set; }
        public SqlExecuteType ExecuteType { get; private set; }
        public long Elapsed { get; private set; }
        public IDbCommand Command { get; private set; }

        public string CommandText
        {
            get
            {
                return Command.CommandText.CloseWhitespace();
            }
        }

        public DbParameterCollection Parameters
        {
            get
            {
                return (DbParameterCollection)Command.Parameters;
            }
        }

        public TraceDbProfilerEventArgs(Stopwatch stopwatch, DateTime date, IDbCommand command, SqlExecuteType executeType)
        {
            Date = date;
            Elapsed = stopwatch == null ? 0 : stopwatch.ElapsedMilliseconds;
            ExecuteType = executeType;
            Command = command;
        }
    }
}
