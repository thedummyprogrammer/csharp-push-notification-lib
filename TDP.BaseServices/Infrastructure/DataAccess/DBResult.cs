//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System.Collections.Generic;
using System.Data;

namespace TDP.BaseServices.Infrastructure.DataAccess
{
    public class DBResult
    {
        public DBResult(int returnValue, DataTable table, List<DataTable> tables, Dictionary<string, object> outputParameters)
        {
            ReturnValue = returnValue;
            Table = table;
            Tables = tables;
            OutputParameters = outputParameters;
        }

        public int ReturnValue { get; private set; }

        public DataTable Table {get; set; }
        public bool TableHasData { get { return Table != null && Table.Rows.Count > 0; } }

        public object TableFirstRow(string columnName)
        {
            object Result = null;
            if (TableHasData)
            {
                Result = Table.Rows[0][columnName];
            }

            return Result;
        }

        public List<DataTable> Tables { get; private set; }

        public Dictionary<string, object> OutputParameters { get; private set; }
    }
}