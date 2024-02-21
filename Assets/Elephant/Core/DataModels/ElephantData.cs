using System;

namespace ElephantSDK
{
    [Serializable]
    public class ElephantData
    {
        public string data;
        public long current_session_id;
        public bool is_offline;
        public bool is_fail;

        public ElephantData(string data, long sessionId, bool isOffline = false, bool isFail = false)
        {
            this.current_session_id = sessionId;
            this.data = data;
            this.is_offline = isOffline;
            this.is_fail = isFail;
        }
    }
}