using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Data;

namespace KOILib.Common.DataAccess.Trace
{
    internal class TraceDbProfiler
        : IDbProfiler
    {
        public event EventHandler<TraceDbProfilerEventArgs> ExecuteBeginning;
        public event EventHandler<TraceDbProfilerErrorEventArgs> ErrorOccurred;
        public event EventHandler<TraceDbProfilerEventArgs> ExecuteFinished;
        public event EventHandler<TraceDbProfilerEventArgs> ReaderFinished;

        private Stopwatch _stopwatch;
        private IDbCommand _command;

        public bool IsActive
        {
            get { return true; }
        }

        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            _stopwatch = Stopwatch.StartNew();

            var e = new TraceDbProfilerEventArgs(
                stopwatch: null,
                date: DateTime.Now,
                command: profiledDbCommand,
                executeType: executeType
            );

            //event raise
            if (ExecuteBeginning != null)
                ExecuteBeginning.Invoke(this, e);
        }

        public void OnError(IDbCommand profiledDbCommand, SqlExecuteType executeType, Exception exception)
        {
            _stopwatch.Stop();

            var e = new TraceDbProfilerErrorEventArgs(
                stopwatch: _stopwatch,
                date: DateTime.Now,
                command: profiledDbCommand,
                executeType: executeType,
                exception: exception
            );

            //event raise
            if (ErrorOccurred != null)
                ErrorOccurred.Invoke(this, e);
        }

        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, DbDataReader reader)
        {
            _command = profiledDbCommand;

            if (executeType != SqlExecuteType.Reader)
            {
                _stopwatch.Stop();

                var e = new TraceDbProfilerEventArgs(
                    stopwatch: _stopwatch,
                    date: DateTime.Now,
                    command: profiledDbCommand,
                    executeType: executeType
                );

                //event raise
                if (ExecuteFinished != null)
                    ExecuteFinished.Invoke(this, e);
            }
        }

        public void ReaderFinish(IDataReader reader)
        {
            _stopwatch.Stop();

            var e = new TraceDbProfilerEventArgs(
                stopwatch: _stopwatch,
                date: DateTime.Now,
                command: _command,
                executeType: SqlExecuteType.Reader
            );

            //event raise
            if (ReaderFinished != null)
                ReaderFinished.Invoke(this, e);
        }
    }
}
