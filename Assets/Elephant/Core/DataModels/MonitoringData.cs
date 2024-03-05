using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class MonitoringData : BaseData
    {
        // TODO add cpu and memory related fields
        public List<double> fpsLogs;
        public List<int> currentLevelLog;
        public int systemMemory;
        public int memoryUsage;
        public int memoryUsagePercentage;
        public float sessionStartBatteryLevel;
        public float sessionEndBatteryLevel;
        
        private MonitoringData()
        {
            fpsLogs = new List<double>();
            fpsLogs.AddRange(MonitoringUtils.GetInstance().GetFpsSessionLog());
            
            currentLevelLog = new List<int>();
            currentLevelLog.AddRange(MonitoringUtils.GetInstance().GetCurrentLevelLog());

            systemMemory = SystemInfo.systemMemorySize;
            memoryUsage = MonitoringUtils.GetInstance().GetMemoryUsage();
            memoryUsagePercentage = MonitoringUtils.GetInstance().GetMemoryUsagePercentage();

            sessionStartBatteryLevel = MonitoringUtils.GetInstance().GetSessionStartBatteryLevel();
            sessionEndBatteryLevel = MonitoringUtils.GetInstance().GetSessionEndBatteryLevel();
            
            MonitoringUtils.GetInstance().Flush();
        }
        
        public static MonitoringData CreateMonitoringData()
        {
            var monitoringData = new MonitoringData();
            monitoringData.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            return monitoringData;
        }
    }
}