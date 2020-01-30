//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System.Collections.Generic;

namespace TDP.BaseServices.Infrastructure.DataAccess
{
    public class DBResult<T> where T : class
    {
        public DBResult(int returnValue, List<T> table, Dictionary<string, object> outputParameters)
        {
            ReturnValue = returnValue;
            Table = table;
            OutputParameters = outputParameters;
        }

        public int ReturnValue { get; private set; }

        public List<T> Table {get; set; }
        public bool TableHasData { get { return Table != null && Table.Count > 0; } }

        public T TableFirstRow()
        {
            T Result = null;
            if (TableHasData)
            {
                Result = Table[0];
            }

            return Result;
        }

        public Dictionary<string, object> OutputParameters { get; private set; }
    }
}