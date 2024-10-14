using System.IO;

namespace Kuci.Logger
{
    using UnityEngine;

    public class LogSaver : MonoBehaviour
    {
        [Header("Right click to the LogSaver.cs component and click 'Save Log History'")]
        [SerializeField] private string _logHistoryFileName = "LogHistory";
        [SerializeField] private string _logHistoryTag = "all";

        [ContextMenu("Save Log History")]
        private void SaveLogHistory()
        {
            if (string.IsNullOrEmpty(_logHistoryTag))
            {
                _logHistoryTag = "all";
            }
            
            var filePath = Path.Combine(Application.persistentDataPath, $"{_logHistoryFileName}.txt");
            DevLogger.SaveHistoryToFile(filePath, _logHistoryTag);
        }
    }
}