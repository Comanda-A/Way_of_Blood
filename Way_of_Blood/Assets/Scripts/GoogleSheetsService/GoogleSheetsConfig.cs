using System;
using System.Collections.Generic;
using UnityEngine;

namespace WayOfBlood.GoogleSheetsService
{
    [CreateAssetMenu(fileName = "GoogleSheetsConfig", menuName = "Configs/GoogleSheetsConfig")]
    public class GoogleSheetsConfig : ScriptableObject
    {
        [Serializable]
        public class SheetEntry
        {
            public string tableName; // Название таблицы
            public string sheetUrl;  // Ссылка на таблицу
        }

        public List<SheetEntry> sheets = new List<SheetEntry>();

        public string GetSheetUrl(string tableName)
        {
            var entry = sheets.Find(sheet => sheet.tableName == tableName);
            return entry != null ? entry.sheetUrl : null;
        }
    }
}
