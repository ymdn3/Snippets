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
        public int Execute(string sql, object param = null, CommandType? commandType = null)
        {
            return Connection.Execute(sql, param, Transaction, CommandTimeout, commandType);
        }
        public object ExecuteScalar(string sql, object param = null, CommandType? commandType = null)
        {
            return Connection.ExecuteScalar(sql, param, Transaction, CommandTimeout, commandType);
        }
        public T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return Connection.ExecuteScalar<T>(sql, param, Transaction, CommandTimeout, commandType);
        }
        public IDataReader ExecuteReader(string sql, object param = null, CommandType? commandType = null)
        {
            return Connection.ExecuteReader(sql, param, Transaction, CommandTimeout, commandType);
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, CommandType? commandType = null)
        {
            return Connection.Query(sql, param, Transaction, buffered, CommandTimeout, commandType);
        }
        public dynamic QueryFirst(string sql, object param = null, CommandType? commandType = null)
        {
            return this.Query(sql, param, true, commandType).FirstOrDefault();
        }
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, CommandType? commandType = null)
        {
            return Connection.Query<T>(sql, param, Transaction, buffered, CommandTimeout, commandType);
        }
        public T QueryFirst<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return this.Query<T>(sql, param, true, commandType).FirstOrDefault();
        }
        public IEnumerable<object> Query(Type type, string sql, object param = null, bool buffered = true, CommandType? commandType = null)
        {
            return Connection.Query(type, sql, param, Transaction, buffered, CommandTimeout, commandType);
        }
        public object QueryFirst(Type type, string sql, object param = null, CommandType? commandType = null)
        {
            return this.Query(type, sql, param, true, commandType).FirstOrDefault();
        }

    }
}
