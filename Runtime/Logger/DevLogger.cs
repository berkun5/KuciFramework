using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kuci.Logger
{
    public static class DevLogger
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // History can be used for debug list view
        private static readonly List<LogEntity> History = new();
#endif        

        public static void Log(object text, string tag = "Log", bool isImportant = false, bool isSilent = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!isSilent)
            {
                Debug.Log($"<color=#00E8FF>DevLog:</color> {text}");
            }

            AddLogToHistory(text, tag, Color.white);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }

        public static void LogWarning(object text, string tag = "Warning", bool isImportant = false, bool isSilent = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!isSilent)
            {
                Debug.LogWarning($"<color=#FFD724>DevLogWarning:</color> {text}");
            }

            AddLogToHistory(text, "tag", Color.yellow);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }

        public static void LogError(object text, string tag = "Error", bool isImportant = false, bool isSilent = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!isSilent)
            {
                Debug.LogError($"<color=#FF3A00>DevLogError:</color> {text}");
            }

            AddLogToHistory(text, tag, Color.red);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static void AddLogToHistory(object text, string tag, Color color)
        {
            History.Add(new LogEntity($"[{History.Count}] :: [{tag}] :: {text}", color));
        }
#endif
        
        public static void SaveHistoryToFile(string filePath, string tag = "all")
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            using (var writer = new StreamWriter(filePath, false)) // 'false' to overwrite existing file
            {
                foreach (var log in History)
                {
                    // Check if the log contains the specified tag, or if "all" is specified, add everything
                    if (tag == "all" || log.Text.Contains($"[{tag}]"))
                    {
                        writer.WriteLine(log.Text);
                    }
                }
            }
            Debug.Log($"History saved to {filePath} with tag: {tag}");
#endif
        }
    }
}
