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
        /// SQLログテキストを構築します
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string BuildSQLLog(TraceDbProfilerEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[SQL:{0:X" + Consts.INSTANCE_HASHCODE_LEN + "}] ", e.Command.GetHashCode());

            sb.Append(e.CommandText);

            //パラメータ
            if (e.Parameters.Count > 0)
                sb.AppendFormat("/*{{{0}}}*/", e.Parameters.ToLogString());

            return sb.ToString();
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
            if (Transaction != null)
                throw new Exception("複数のトランザクションを開始することはできません。");

            //connection open
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            //begin tran
            Transaction = Connection.BeginTransaction();

            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:X" + Consts.INSTANCE_HASHCODE_LEN + "}] Transaction Begins.", Transaction.GetHashCode());
            }

            var wrapper = new DbContextTransaction(this);
            wrapper.Committed += Transaction_Committed;
            wrapper.Rollbacked += Transaction_Rollbacked;
            return wrapper;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (Transaction != null)
                throw new Exception("複数のトランザクションを開始することはできません。");

            //connection open
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            //begin tran
            Transaction = Connection.BeginTransaction(il);

            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:X" + Consts.INSTANCE_HASHCODE_LEN + "}] Transaction Begins.", Transaction.GetHashCode());
            }

            var wrapper = new DbContextTransaction(this);
            wrapper.Committed += Transaction_Committed;
            wrapper.Rollbacked += Transaction_Rollbacked;
            return wrapper;
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
                //ロールバック保証
                try
                {
                    Transaction.Rollback();
                }
                catch (Exception)
                {                    
                }
                Transaction.Dispose();
                Transaction = null;
            }

            if (this is ILog4Logging)
            {
                var prof = (TraceDbProfiler)Connection.Profiler;
                prof.ErrorOccurred -= DbContext_ErrorOccurred;
                prof.ExecuteFinished -= DbContext_ExecuteFinished;
                prof.ReaderFinished -= DbContext_ReaderFinished;
            }
            ((IDbConnection)Connection).Dispose();
        }
        #endregion

        protected void InitConnection(DbConnection conn)
        {
            var prof = default(TraceDbProfiler);
            if (this is ILog4Logging)
            {
                prof = new TraceDbProfiler();
                prof.ErrorOccurred += DbContext_ErrorOccurred;
                prof.ExecuteFinished += DbContext_ExecuteFinished;
                prof.ReaderFinished += DbContext_ReaderFinished;
            }
            Connection = new ProfiledDbConnection(conn, prof);
        }
        
        private void DbContext_ReaderFinished(object sender, TraceDbProfilerEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, BuildSQLLog(e));
            }
        }

        private void DbContext_ExecuteFinished(object sender, TraceDbProfilerEventArgs e)
        {
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, BuildSQLLog(e));
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
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:X" + Consts.INSTANCE_HASHCODE_LEN + "}] Transaction Rollback.", Transaction.GetHashCode());
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
                    logging.Logger.Write(logging.LogLevel, "[TRN:{0:X" + Consts.INSTANCE_HASHCODE_LEN + "}] Transaction Commit.", Transaction.GetHashCode());
            }

            Transaction = null;
        }


    }
}
