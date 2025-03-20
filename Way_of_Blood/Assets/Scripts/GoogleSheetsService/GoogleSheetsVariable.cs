using System;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class GoogleSheetsVariable : PropertyAttribute
{
    public string tableName;
    public string rowKey;
    public string columnName;

    public GoogleSheetsVariable(string tableName, string rowKey, string columnName)
    {
        this.tableName = tableName;
        this.rowKey = rowKey;
        this.columnName = columnName;
    }
}
