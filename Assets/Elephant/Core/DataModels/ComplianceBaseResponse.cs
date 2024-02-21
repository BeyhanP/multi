using System;

namespace ElephantSDK
{
    [Serializable]
    public class ComplianceBaseResponse
    {
        public bool required;
        public string title;
        public string content;
        public string privacy_policy_text;
        public string privacy_policy_url;
        public string terms_of_service_text;
        public string terms_of_service_url;

        public ComplianceBaseResponse()
        {
            this.required = false;
            this.title = "";
            this.content = "";
            this.privacy_policy_text = "";
            this.privacy_policy_url = "";
            this.terms_of_service_text = "";
            this.terms_of_service_url = "";
        }
    }
}