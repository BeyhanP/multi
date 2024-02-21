using UnityEngine;

namespace ElephantSDK
{
    public class ElephantLog
    {
        private static ElephantLog instance;
        
        private static bool isLoggingEnabled;

        public static ElephantLog GetInstance(ElephantLogLevel logLevel)
        {
            return instance ?? (instance = new ElephantLog(logLevel));
        }
        
        private ElephantLog(ElephantLogLevel logLevel)
        {
            switch (logLevel)
            {
                case ElephantLogLevel.Debug:
                {
                    isLoggingEnabled = true;
                    break;
                }
                case ElephantLogLevel.Prod:
                {
                    isLoggingEnabled = false;
                    break;
                }
                default:
                    isLoggingEnabled = false;
                    break;
            }
        }
        
        public static void Log(string filter, string message)
        {
            if (isLoggingEnabled)
            {
                Debug.Log("<" + filter + "> " + message);
            }
        }
        
        public static void LogError(string filter, string message)
        {
            if (isLoggingEnabled)
            {
                Debug.LogError("<" + filter + "> " + message);
            }
        }
    }

    public enum ElephantLogLevel
    {
        Debug = 1,
        Prod
    }
}