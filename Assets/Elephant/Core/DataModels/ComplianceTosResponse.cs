using System;

namespace ElephantSDK
{
    [Serializable]
    public class ComplianceTosResponse: ComplianceBaseResponse
    {
        public string consent_text_action_button;

        public ComplianceTosResponse()
        {
            this.consent_text_action_button = "";
        }
    }
}