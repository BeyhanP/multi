using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    public class MonitoringUtils
    {
        private static MonitoringUtils _instance;

        private readonly List<double> _fpsSessionLog;
        private readonly List<int> _currentLevelLog;

        private int _currentLevel;
        private int _memoryUsage;
        private int _memoryUsagePercentage;

        private MonitoringUtils()
        {
            _fpsSessionLog = new List<double>();
            _currentLevelLog = new List<int>();
            _currentLevel = -1;
            _memoryUsage = 0;
            _memoryUsagePercentage = 0;
        }

        public static MonitoringUtils GetInstance()
        {
            return _instance ?? (_instance = new MonitoringUtils());
        }

        public void LogFps(double fpsValue)
        {
            _fpsSessionLog.Add(fpsValue);
        }
        
        public float CalculateFps(float[] fpsBuffer)
        {
            float total = 0;

            foreach (var v in fpsBuffer)
            {
                total += v;
            }

            return Mathf.Round(total / fpsBuffer.Length);
        }

        public void LogCurrentLevel()
        {
            _currentLevelLog.Add(_currentLevel);
        }

        public List<double> GetFpsSessionLog()
        {
            return _fpsSessionLog;
        }
        
        public List<int> GetCurrentLevelLog()
        {
            return _currentLevelLog;
        }

        public void SetCurrentLevel(int currentLevel)
        {
            _currentLevel = currentLevel;
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }
        
        public void SetMemoryUsage(int memoryUsageValue)
        {
            _memoryUsage = memoryUsageValue;
        }
        
        public int GetMemoryUsage()
        {
            return _memoryUsage;
        }
        
        public void SetMemoryUsagePercentage(int memoryUsagePercentage)
        {
            _memoryUsagePercentage = memoryUsagePercentage;
        }
        
        public int GetMemoryUsagePercentage()
        {
            return _memoryUsagePercentage;
        }

        public void Flush()
        {
            _fpsSessionLog?.Clear();
            _currentLevelLog?.Clear();
        }
    }
}