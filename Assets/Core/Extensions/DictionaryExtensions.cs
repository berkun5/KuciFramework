using System.Collections.Generic;
using Kuci.Logger;

namespace Kuci.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool GetBool(this IDictionary<string, object> dictionary, string key, bool defaultValue = false,
            bool logErrorIfDefaultUsed = false)
        {
            if (!dictionary.ContainsKey(key) || !bool.TryParse(dictionary[key].ToString(), out var result))
            {
                if (logErrorIfDefaultUsed)
                {
                    DevLogger.LogWarning($"Failed to retrieve value for {key} from dictionary. " +
                                         $"Default value {defaultValue} is used");
                }

                return defaultValue;
            }

            return result;
        }

        public static int GetInt(this IDictionary<string, object> dictionary, string key, int defaultValue = 0,
            bool logErrorIfDefaultUsed = false)
        {
            if (!dictionary.ContainsKey(key) || !int.TryParse(dictionary[key].ToString(), out var result))
            {
                if (logErrorIfDefaultUsed)
                {
                    DevLogger.LogWarning($"Failed to retrieve value for {key} from dictionary. " +
                                         $"Default value {defaultValue} is used");
                }

                return defaultValue;
            }

            return result;
        }

        public static float GetFloat(this IDictionary<string, object> dictionary, string key, float defaultValue = 0f,
            bool logErrorIfDefaultUsed = false)
        {
            if (!dictionary.ContainsKey(key) || !float.TryParse(dictionary[key].ToString(), out var result))
            {
                if (logErrorIfDefaultUsed)
                {
                    DevLogger.LogWarning($"Failed to retrieve value for {key} from dictionary. " +
                                         $"Default value {defaultValue} is used");
                }

                return defaultValue;
            }

            return result;
        }

        public static string GetString(this IDictionary<string, object> dictionary, string key,
            string defaultValue = "", bool logErrorIfDefaultUsed = false)
        {
            if (!dictionary.ContainsKey(key))
            {
                if (logErrorIfDefaultUsed)
                {
                    DevLogger.LogWarning($"Failed to retrieve value for {key} from dictionary. " +
                                         $"Default value {defaultValue} is used");
                }

                return defaultValue;
            }

            return dictionary[key].ToString();
        }
    }
}