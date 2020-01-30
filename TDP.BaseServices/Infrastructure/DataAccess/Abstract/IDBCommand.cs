//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Data;

namespace TDP.BaseServices.Infrastructure.DataAccess.Abstract
{
    public interface IDBCommand
    {
        void Init(CommandType commandType, string cmdText, IDbConnection connection, IDbTransaction transaction);

        IDBCommand Parameter(string paramName, string value, bool output = false);
        IDBCommand Parameter(string paramName, int? value, bool output = false);
        IDBCommand Parameter(string paramName, int value, bool output = false);
        IDBCommand Parameter(string paramName, long? value, bool output = false);
        IDBCommand Parameter(string paramName, long value, bool output = false);
        IDBCommand Parameter(string paramName, decimal? value, bool output = false);
        IDBCommand Parameter(string paramName, decimal value, bool output = false);
        IDBCommand Parameter(string paramName, DateTime? value, bool output = false);
        IDBCommand Parameter(string paramName, DateTime value, bool output = false);
        IDBCommand Parameter(string paramName, bool? value, bool output = false);
        IDBCommand Parameter(string paramName, bool value, bool output = false);
        IDBCommand Parameter(string paramName, Guid? value, bool output = false);
        IDBCommand Parameter(string paramName, Guid value, bool output = false);

        DBResult Execute();
        DBResult<T> ExecuteT<T>() where T : class, new();
    }
}
