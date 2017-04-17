using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace KOILib.Common.DataAccess
{
	/// <summary>
    /// Dapper Caller Inerfaces
    /// </summary>
    public abstract partial class DbContextBase
    {
        /// <summary>
        /// 実行ロックオブジェクト
        /// </summary>
        private readonly object lockingConn = new object();

        public int Execute(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.Execute(sql, param, Transaction, timeout ?? CommandTimeout, commandType);
            }
        }
        public object ExecuteScalar(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.ExecuteScalar(sql, param, Transaction, timeout ?? CommandTimeout, commandType);
            }
        }
        public T ExecuteScalar<T>(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.ExecuteScalar<T>(sql, param, Transaction, timeout ?? CommandTimeout, commandType);
            }
        }
        public IDataReader ExecuteReader(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.ExecuteReader(sql, param, Transaction, timeout ?? CommandTimeout, commandType);
            }
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.Query(sql, param, Transaction, buffered, timeout ?? CommandTimeout, commandType);
            }
        }
        public dynamic QueryFirst(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return this.Query(sql, param, true, timeout, commandType).FirstOrDefault();
            }
        }
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.Query<T>(sql, param, Transaction, buffered, timeout ?? CommandTimeout, commandType);
            }
        }
        public T QueryFirst<T>(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return this.Query<T>(sql, param, true, timeout, commandType).FirstOrDefault();
            }
        }
        public IEnumerable<object> Query(Type type, string sql, object param = null, bool buffered = true, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return Connection.Query(type, sql, param, Transaction, buffered, timeout ?? CommandTimeout, commandType);
            }
        }
        public object QueryFirst(Type type, string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            lock (lockingConn)
            {
                return this.Query(type, sql, param, true, timeout, commandType).FirstOrDefault();
            }
        }

    }
}
