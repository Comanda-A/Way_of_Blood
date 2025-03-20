using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GoogleSheetsBinder : MonoBehaviour
{
    private void Start()
    {
        BindVariables();
    }

    private void BindVariables()
    {
        var components = GetComponents<MonoBehaviour>();

        foreach (var component in components)
        {
            var fields = component.GetType().GetFields();

            foreach (var field in fields)
            {
                var attribute = (GoogleSheetsVariable)System.Attribute.GetCustomAttribute(field, typeof(GoogleSheetsVariable));

                if (attribute != null)
                {
                    GoogleSheetsService.Instance.Subscribe(attribute.tableName, data =>
                    {
                        ApplyValue(component, field, attribute, data);
                    });

                    GoogleSheetsService.Instance.LoadSheet(attribute.tableName, data =>
                    {
                        ApplyValue(component, field, attribute, data);
                    });
                }
            }
        }
    }

    private void ApplyValue(object target, FieldInfo field, GoogleSheetsVariable attribute, List<Dictionary<string, string>> data)
    {
        foreach (var row in data)
        {
            if (row.ContainsKey("Key") && row["Key"] == attribute.rowKey && row.ContainsKey(attribute.columnName))
            {
                field.SetValue(target, ConvertValue(field.FieldType, row[attribute.columnName]));
                Debug.Log($"new value: {field.GetValue(target)}");
                break;
            }
        }
    }

    private object ConvertValue(System.Type type, string value)
    {
        if (type == typeof(int)) return int.Parse(value);
        if (type == typeof(float)) return float.Parse(value);
        if (type == typeof(bool)) return bool.Parse(value);
        return value;
    }
}
