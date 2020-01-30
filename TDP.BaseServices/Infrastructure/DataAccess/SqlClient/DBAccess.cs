//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Data.SqlClient;
using TDP.BaseServices.Infrastructure.DataAccess.Abstract;
using TDP.BaseServices.Infrastructure.Configuration.Abstract;

namespace TDP.BaseServices.Infrastructure.DataAccess.SqlClient
{
    public class DBAccess : IDBAccess
    {
        private const string _connectionStringKey = "DBConnectionString";
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private IConfigReader _config;

        private void DisposeConnection()
        {
            try
            {
                if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            catch { }
        }

        public DBAccess(IConfigReader configReader) : this(configReader, false)
        {
        }

        protected DBAccess(IConfigReader configReader, bool openConnection)
        {
            _config = configReader;

            string ConnectionString = _config.GetConnectionString(_connectionStringKey);
            _connection = new SqlConnection(ConnectionString);

            if (openConnection)
                _connection.Open();
        }

        IDBCommand IDBAccess.Command(string text)
        {
            DBCommand Command = new DBCommand();
            Command.Init(CommandType.Command, text, _connection, _transaction);
            return Command;
        }

        IDBCommand IDBAccess.Query(string text)
        {
            DBCommand Command = new DBCommand();
            Command.Init(CommandType.Query, text, _connection, _transaction);
            return Command;
        }

        IDBCommand IDBAccess.StoredProcedure(string spName)
        {
            DBCommand Command = new DBCommand();
            Command.Init(CommandType.StoredProcedures, spName, _connection, _transaction);
            return Command;
        }

        IDBAccess IDBAccess.GetDBAccess()
        {
            return new DBAccess(_config, true);
        }

        void IDBAccess.BeginTran()
        {
            if (_transaction == null)
                _transaction = _connection.BeginTransaction();
        }

        void IDBAccess.RollbackTran()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        void IDBAccess.CommitTran()
        {
            if (_transaction != null)
                _transaction.Commit();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                DisposeConnection();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DBAccess() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}