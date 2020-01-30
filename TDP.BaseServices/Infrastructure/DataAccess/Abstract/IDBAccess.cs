//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;

namespace TDP.BaseServices.Infrastructure.DataAccess.Abstract
{
    public interface IDBAccess : IDisposable
    {
        IDBAccess GetDBAccess();
        void BeginTran();
        void RollbackTran();
        void CommitTran();

        IDBCommand Query(string text);
        IDBCommand Command(string text);
        IDBCommand StoredProcedure(string spName);
    }
}
