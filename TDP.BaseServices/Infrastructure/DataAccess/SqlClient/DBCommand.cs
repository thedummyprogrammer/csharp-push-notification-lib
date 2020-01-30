//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TDP.BaseServices.Infrastructure.DataAccess.Abstract;

namespace TDP.BaseServices.Infrastructure.DataAccess.SqlClient
{
    public class DBCommand : IDBCommand
    {
        private const string _retValueParName = "@RetValue";
        private CommandType _commandType;
        private SqlConnection _connection;
        private SqlCommand _command;

        private static T GetObjectFromReaderRecord<T>(IDataReader reader) where T : new()
        {
            T item = new T();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string ColumnName;
                DBColumnNameAttribute Attr = (DBColumnNameAttribute)propertyInfo.GetCustomAttribute(typeof(DBColumnNameAttribute));
                
                if (Attr != null && Attr.ColumnName != string.Empty)
                {
                    ColumnName = Attr.ColumnName;
                    propertyInfo.SetValue(item, reader[Attr.ColumnName], null);
                }
            }

            return item;
        }

        public static T GetObjectFromReader<T>(IDataReader reader) where T : new()
        {
            if (reader.Read())
                return GetObjectFromReaderRecord<T>(reader);
            return default(T);
        }

        public static List<T> GetObjectListFromReader<T>(IDataReader reader) where T : new()
        {
            List<T> list = new List<T>();
            while (reader.Read())
                list.Add(GetObjectFromReaderRecord<T>(reader));
            return list;
        }

        private DBResult ExecuteCommand()
        {
            int ReturnValue = _command.ExecuteNonQuery();
            DBResult Result = new DBResult(ReturnValue, null, null, null);
            return Result;
        }

        private DBResult ExecuteQuery()
        {
            DataSet DsResult = new DataSet();
            SqlDataAdapter DA = new SqlDataAdapter(_command);
            DA.Fill(DsResult);

            
            DataTable Table = null;
            if (DsResult.Tables.Count > 0)
                Table = DsResult.Tables[0];

            List<DataTable> Tables = new List<DataTable>();
            foreach (DataTable Tb in DsResult.Tables)
            {
                Tables.Add(Tb);
            }
            
            DBResult Result = new DBResult(0, Table, Tables, null);
            return Result;
        }

        private DBResult<T> ExecuteQueryT<T>() where T : class, new()
        {
            IDataReader Reader = _command.ExecuteReader();
            List<T> Table = GetObjectListFromReader<T>(Reader);

            DBResult<T> Result = new DBResult<T>(0, Table, null);
            return Result;
        }

        private DBResult ExecuteStoredProcedure()
        {
            DataSet DsResult = new DataSet();
            SqlDataAdapter DA = new SqlDataAdapter(_command);
            DA.Fill(DsResult);

            int ReturnValue = Convert.ToInt32(_command.Parameters[_retValueParName].Value);

            DataTable Table = null;
            if (DsResult.Tables.Count > 0)
                Table = DsResult.Tables[0];

            List<DataTable> Tables = new List<DataTable>();
            foreach (DataTable Tb in DsResult.Tables)
            {
                Tables.Add(Tb);
            }

            Dictionary<string, object> OutputParams = new Dictionary<string, object>();
            foreach (SqlParameter Par in _command.Parameters)
            {
                if (Par.Direction == ParameterDirection.InputOutput)
                {
                    OutputParams.Add(Par.ParameterName, Par.Value);
                }
            }

            DBResult Result = new DBResult(ReturnValue, Table, Tables, OutputParams);
            return Result;
        }

        private DBResult<T> ExecuteStoredProcedureT<T>() where T : class, new()
        {
            IDataReader Reader = _command.ExecuteReader();
            List<T> Table = GetObjectListFromReader<T>(Reader);

            int ReturnValue = Convert.ToInt32(_command.Parameters[_retValueParName].Value);

            Dictionary<string, object> OutputParams = new Dictionary<string, object>();
            foreach (SqlParameter Par in _command.Parameters)
            {
                if (Par.Direction == ParameterDirection.InputOutput)
                {
                    OutputParams.Add(Par.ParameterName, Par.Value);
                }
            }

            DBResult<T> Result = new DBResult<T>(ReturnValue, Table, OutputParams);
            return Result;
        }

        public void Init(CommandType commandType, string cmdText, IDbConnection connection, IDbTransaction transaction)
        {
            if (string.IsNullOrEmpty(cmdText))
                throw new ArgumentNullException("cmdText");

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (commandType != CommandType.Command && commandType != CommandType.Query && commandType != CommandType.StoredProcedures)
                throw new ArgumentException("Invalid value", "commandType");

            _commandType = commandType;
            _connection = (SqlConnection)connection;
            _command = new SqlCommand(cmdText, (SqlConnection)connection, (SqlTransaction)transaction);

            switch (commandType)
            {
                case CommandType.Command:
                case CommandType.Query:
                    _command.CommandType = System.Data.CommandType.Text;
                    break;

                case CommandType.StoredProcedures:
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add(_retValueParName, SqlDbType.Int);
                    _command.Parameters[_retValueParName].Direction = ParameterDirection.ReturnValue;
                    break;
            }
        }

        public DBResult Execute()
        {
            DBResult Result = null;
            bool OpenConnection = (_connection.State == ConnectionState.Closed);

            if (OpenConnection)
                _connection.Open();

            try
            {
                if (_commandType == CommandType.Command)
                    Result = ExecuteCommand();
                else if (_commandType == CommandType.Query)
                    Result = ExecuteQuery();
                else if (_commandType == CommandType.StoredProcedures)
                    Result = ExecuteStoredProcedure();
            }
            catch { throw; }
            finally
            {
                try
                {
                    if (OpenConnection)
                        _connection.Close();
                }
                catch { }
            }

            return Result;
        }

        public DBResult<T> ExecuteT<T>() where T : class, new()
        {
            DBResult<T> Result = null;
            bool OpenConnection = (_connection.State == ConnectionState.Closed);

            if (OpenConnection)
                _connection.Open();

            try
            {
                if (_commandType == CommandType.Command)
                    throw new ApplicationException("Cannot execute a command using ExecuteT method.");
                else if (_commandType == CommandType.Query)
                    Result = ExecuteQueryT<T>();
                else if (_commandType == CommandType.StoredProcedures)
                    Result = ExecuteStoredProcedureT<T>();
            }
            catch { throw; }
            finally
            {
                try
                {
                    if (OpenConnection)
                        _connection.Close();
                }
                catch { }
            }

            return Result;
        }

        public IDBCommand Parameter(string paramName, string value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.VarChar);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, int? value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Int);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, int value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Int);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, long? value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.BigInt);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, long value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.BigInt);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, decimal? value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Decimal);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, decimal value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Decimal);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, DateTime? value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.DateTime);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, DateTime value, bool output = false)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");

            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.DateTime);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, bool? value, bool output = false)
        {
            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Bit);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, bool value, bool output = false)
        {
            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.Bit);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, Guid? value, bool output = false)
        {
            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.UniqueIdentifier);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;

            if (value == null)
                Param.Value = DBNull.Value;
            else
                Param.Value = value;

            return this;
        }

        public IDBCommand Parameter(string paramName, Guid value, bool output = false)
        {
            SqlParameter Param = _command.Parameters.Add(paramName, SqlDbType.UniqueIdentifier);
            Param.Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input;
            Param.Value = value;

            return this;
        }

    }
}