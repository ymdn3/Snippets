using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common.Core;
using KOILib.Common.DataAccess.Trace;
using KOILib.Common.Log4;
using StackExchange.Profiling.Data;

namespace KOILib.Common.DataAccess
{
    public abstract class DbContextBase<TConnection>
        : DbContextBase
        where TConnection : DbConnection, new()
    {
        #region Static Members
        public static TReturn Create<TReturn>()
            where TReturn : DbContextBase<TConnection>, new()
        {
            //コンテキスト生成
            var context = new TReturn();

            //コネクション生成とトレースイベントの紐付け
            var conn = new TConnection();
            context.InitConnection(conn);

            return context;
        }
        #endregion
    }

    public abstract partial class DbContextBase
        : IDbConnection, IDisposable
    {
        #region Static Members
        /// <summary>
        /// 接続文字列を編集します。
        /// 値がnullのとき削除します。
        /// </summary>
        /// <param name="srcConnstr"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EditConnectionString(string srcConnstr, string key, object value)
        {
            var cb = new DbConnectionStringBuilder();
            cb.ConnectionString = srcConnstr;
            if (cb.ContainsKey(key))
                cb.Remove(key);
            if (value != null)
                cb.Add(key, value);
            return cb.ToString();
        }

        /// <summary>
        /// SQLログテキストを構築します（標準）
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string BuildSQLLog(TraceDbProfilerEventArgs e)
        {
            var sb = new StringBuilder();
            SQLLogAppendHead(sb, e);
            sb.Append(" ");
            SQLLogAppendCommand(sb, e);
            sb.Append(" ");
            SQLLogAppendParameter(sb, e);
            return sb.ToString();
        }
        /// <summary>
        /// SQL実行済みログを構築します
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string BuildSQLLogFinish(TraceDbProfilerEventArgs e)
        {
            var sb = new StringBuilder();
            SQLLogAppendHead(sb, e);
            sb.Append(" ");
            SQLLogAppendStopwatch(sb, e);
            return sb.ToString();
        }
        private static void SQLLogAppendHead(StringBuilder sb, TraceDbProfilerEventArgs e)
        {
            sb.AppendFormat("[SQL:{0:x" + Consts.INSTANCE_HASHCODE_LEN + "}]", e.Command.GetHashCode());
        }
        private static void SQLLogAppendCommand(StringBuilder sb, TraceDbProfilerEventArgs e)
        {
            sb.Append(e.CommandText);
        }
        private static void SQLLogAppendParameter(StringBuilder sb, TraceDbProfilerEventArgs e)
        {
            if (e.Parameters.Count > 0)
                sb.AppendFormat("/*{{{0}}}*/", e.Parameters.ToLogString());
        }
        private static void SQLLogAppendStopwatch(StringBuilder sb, TraceDbProfilerEventArgs e)
        {
            sb.AppendFormat("{0} msec elapsed.", e.Elapsed);
        }
        #endregion

        public int? CommandTimeout { get; set; }
        public ProfiledDbConnection Connection { get; private set; }
        protected internal DbTransaction Transaction { get; private set; }

        #region IDbConnection Implements
        public string ConnectionString
        {
            get
            {
                return ((IDbConnection)Connection).ConnectionString;
            }
            set
            {
                ((IDbConnection)Connection).ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return ((IDbConnection)Connection).ConnectionTimeout;
            }
        }

        public string Database
        {
            get
            {
                return ((IDbConnection)Connection).Database;
            }
        }

        public ConnectionState State
        {
            get
            {
                return ((IDbConnection)Connection).State;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified, disposingCommit: false);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return BeginTransaction(il, disposingCommit: false);
        }

        public void ChangeDatabase(string databaseName)
        {
            ((IDbConnection)Connection).ChangeDatabase(databaseName);
        }

        public void Close()
        {
            ((IDbConnection)Connection).Close();
        }

        public IDbCommand CreateCommand()
        {
            return ((IDbConnection)Connection).CreateCommand();
        }

        public void Open()
        {
            ((IDbConnection)Connection).Open();
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            if (this is ILog4Logging)
            {
                var prof = (TraceDbProfiler)Connection.Profiler;
                prof.ExecuteBeginning -= DbContext_ExecuteBeginning;
                prof.ErrorOccurred -= DbContext_ErrorOccurred;
                prof.ExecuteFinished -= DbContext_ExecuteFinished;
                prof.ReaderFinished -= DbContext_ReaderFinished;
            }
            ((IDbConnection)Connection).Dispose();
        }
        #endregion

        public DbConnection InnerConnection
        {
            get { return this.Connection.InnerConnection; }
        }

        protected void InitConnection(DbConnection conn)
        {
            var prof = default(TraceDbProfiler);
            if (this is ILog4Logging)
            {
                prof = new TraceDbProfiler();
                prof.ExecuteBeginning += DbContext_ExecuteBeginning;
                prof.ErrorOccurred += DbContext_ErrorOccurred;
                prof.ExecuteFinished += DbContext_ExecuteFinished;
                prof.ReaderFinished += DbContext_ReaderFinished;
            }
            Connection = new ProfiledDbConnection(conn, prof);
        }

        private void DbContext_ExecuteBeginning(object sender, TraceDbProfilerEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, BuildSQLLog(e));
            }
        }

        private void DbContext_ReaderFinished(object sender, TraceDbProfilerEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null && logging.LogLevel != LogLevel.None)
                    logging.Logger.Write(LogLevel.Debug, BuildSQLLogFinish(e));
            }
        }

        private void DbContext_ExecuteFinished(object sender, TraceDbProfilerEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null && logging.LogLevel != LogLevel.None)
                    logging.Logger.Write(LogLevel.Debug, BuildSQLLogFinish(e));
            }
        }

        private void DbContext_ErrorOccurred(object sender, TraceDbProfilerErrorEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(LogLevel.Error, BuildSQLLog(e));
            }
        }

        private void Transaction_Rollbacked(object sender, EventArgs e)
        {
            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:x" + Consts.INSTANCE_HASHCODE_LEN + "}] ROLLBACK", Transaction.GetHashCode());
            }

            Transaction = null;
        }

        private void Transaction_Committed(object sender, EventArgs e)
        {
            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:x" + Consts.INSTANCE_HASHCODE_LEN + "}] COMMIT", Transaction.GetHashCode());
            }

            Transaction = null;
        }

        private IDbTransaction BeginTransaction(IsolationLevel il, bool disposingCommit)
        {
            if (Transaction != null)
                throw new Exception("複数のトランザクションを開始することはできません。");

            //connection open
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            //begin tran
            if (il == IsolationLevel.Unspecified)
                Transaction = Connection.BeginTransaction();
            else
                Transaction = Connection.BeginTransaction(il);

            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:x" + Consts.INSTANCE_HASHCODE_LEN + "}] BEGIN TRAN", Transaction.GetHashCode());
            }

            var wrapper = new DbContextTransaction(this);
            wrapper.DisposingCommit = disposingCommit;
            wrapper.Committed += Transaction_Committed;
            wrapper.Rollbacked += Transaction_Rollbacked;
            return wrapper;
        }

        public IDbTransaction BeginTransaction(bool disposingCommit)
        {
            return BeginTransaction(IsolationLevel.Unspecified, disposingCommit);
        }


    }
}
