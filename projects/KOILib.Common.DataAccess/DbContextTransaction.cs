using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common;
using KOILib.Common.Log4;

namespace KOILib.Common.DataAccess
{
    internal class DbContextTransaction
        : DbTransaction
    {
        public event EventHandler Committed;
        public event EventHandler Rollbacked;

        public bool DisposingCommit { get; set; }

        private DbContextBase _context;

        private DbTransaction _transaction
        {
            get { return _context.Transaction; }
        }

        #region DbTransaction Overrides
        protected override DbConnection DbConnection
        {
            get { return _transaction.Connection; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return _transaction.IsolationLevel; }
        }

        public override void Commit()
        {
            //commit tran
            _transaction.Commit();

            //event raise
            Committed(this, EventArgs.Empty);
        }

        public override void Rollback()
        {
            //rollback tran
            _transaction.Rollback();

            //event raise
            Rollbacked(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    //ロールバック・コミット保証
                    try
                    {
                        if (DisposingCommit)
                            Commit();
                        else
                            Rollback();
                    }
                    catch (Exception)
                    {
                    }
                }

                _context = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        public DbContextTransaction(DbContextBase context)
        {
            _context = context;
        }
    }
}
