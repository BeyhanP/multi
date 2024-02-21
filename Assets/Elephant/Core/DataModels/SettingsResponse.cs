using System;

namespace ElephantSDK
{
    [Serializable]
    public class SettingsActionResponse
    {
        public string title;
        public string action;
        public string payload;
    }

    [Serializable]
    public class SettingsResponse
    {
        public SettingsActionResponse[] actions;
    }
}