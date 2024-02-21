using System;

namespace ElephantSDK
{
    [Serializable]
    public class ComplianceBlockedResponse
    {
        public bool is_blocked;
        public string title;
        public string content;
        public string warning_text;
        public string button_text;
        
        
        public ComplianceBlockedResponse()
        {
            this.is_blocked = false;
            this.content = "";
            this.title = "";
            this.warning_text = "";
            this.button_text = "";
        }
    }
}