using System.Data;

namespace Aster.Common.Data.Core.Sessions
{
    public class DapperSession : IDapperSession
    {
        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; set; }

        public DapperSession(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Transaction = Connection.BeginTransaction(il);
        }

        public IDbTransaction BeginTransaction()
        {
            return Transaction = Connection.BeginTransaction();
        }

        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            if (Transaction != null)
                Transaction.Commit();

            Connection.Close();
        }

        public string ConnectionString
        {
            get
            {
                return Connection.ConnectionString;
            }
            set
            {
                Connection.ConnectionString = value;
            }
        }

        public int ConnectionTimeout => Connection.ConnectionTimeout;

        public IDbCommand CreateCommand()
        {
            return Connection.CreateCommand();
        }

        public string Database => Connection.Database;

        public void Open()
        {
            Connection.Open();
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public ConnectionState State => Connection.State;

        public void Dispose()
        {
            if (Transaction != null && Transaction.Connection != null && Transaction.Connection.State == ConnectionState.Open)
                Transaction.Commit();

            Connection.Dispose();
        }
    }
}
