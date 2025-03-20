using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace WayOfBlood.GoogleSheetsService
{
    public class GoogleSheetsService : MonoBehaviour
    {
        private static GoogleSheetsService _service;
        private static GoogleSheetsService _instance
        {
            get
            {
                if (_service == null)
                {
                    GameObject obj = GameObject.Find("Google_sheets_service");
                    _service = obj.GetComponent<GoogleSheetsService>();
                    DontDestroyOnLoad(obj);
                }
                return _service;
            }
        }

        public float updateInterval = 30f;

        [SerializeField] private GoogleSheetsConfig config;

        private Dictionary<string, List<Dictionary<string, string>>> cache = new Dictionary<string, List<Dictionary<string, string>>>();
        private Dictionary<string, List<Action<List<Dictionary<string, string>>>>> subscribers = new Dictionary<string, List<Action<List<Dictionary<string, string>>>>>();

        private void Start()
        {
            StartCoroutine(AutoUpdate());
        }

        public void LoadSheet(string tableName, Action<List<Dictionary<string, string>>> callback)
        {
            print(config);
            string sheetUrl = config.GetSheetUrl(tableName);
            if (string.IsNullOrEmpty(sheetUrl))
            {
                Debug.LogError($"Таблица '{tableName}' не найдена в конфиге.");
                return;
            }

            StartCoroutine(FetchSheetData(tableName, sheetUrl, callback));
        }

        private IEnumerator FetchSheetData(string tableName, string sheetUrl, Action<List<Dictionary<string, string>>> callback)
        {
            string csvUrl = sheetUrl.Replace("/edit?gid=", "/gviz/tq?tqx=out:csv&gid=");
            using (UnityWebRequest request = UnityWebRequest.Get(csvUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    List<Dictionary<string, string>> data = ParseCSV(request.downloadHandler.text);

                    if (!cache.ContainsKey(tableName) || !AreListsEqual(cache[tableName], data))
                    {
                        cache[tableName] = data;
                        NotifySubscribers(tableName, data);
                    }

                    callback?.Invoke(data);
                }
                else
                {
                    Debug.LogError($"Ошибка загрузки таблицы '{tableName}': {request.error}");
                }
            }
        }

        private IEnumerator AutoUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(updateInterval);
                foreach (var entry in config.sheets)
                {
                    LoadSheet(entry.tableName, null);
                }
            }
        }

        private List<Dictionary<string, string>> ParseCSV(string csv)
        {
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            string[] lines = csv.Split('\n');

            if (lines.Length < 2) return data;

            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                Dictionary<string, string> entry = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    entry[headers[j].Trim()] = values[j].Trim();
                }

                data.Add(entry);
            }

            return data;
        }

        private bool AreListsEqual(List<Dictionary<string, string>> oldData, List<Dictionary<string, string>> newData)
        {
            if (oldData.Count != newData.Count) return false;
            for (int i = 0; i < oldData.Count; i++)
            {
                foreach (var key in oldData[i].Keys)
                {
                    if (!newData[i].ContainsKey(key) || oldData[i][key] != newData[i][key])
                        return false;
                }
            }
            return true;
        }

        public void Subscribe(string tableName, Action<List<Dictionary<string, string>>> callback)
        {
            if (!subscribers.ContainsKey(tableName))
            {
                subscribers[tableName] = new List<Action<List<Dictionary<string, string>>>>();
            }
            subscribers[tableName].Add(callback);
        }

        private void NotifySubscribers(string tableName, List<Dictionary<string, string>> data)
        {
            if (subscribers.ContainsKey(tableName))
            {
                foreach (var callback in subscribers[tableName])
                {
                    callback?.Invoke(data);
                }
            }
        }
    }
}