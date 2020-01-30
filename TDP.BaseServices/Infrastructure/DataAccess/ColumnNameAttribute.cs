//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;

namespace TDP.BaseServices.Infrastructure.DataAccess
{
    public class DBColumnNameAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public DBColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}