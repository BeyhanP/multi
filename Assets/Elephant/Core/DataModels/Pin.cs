using System;

namespace ElephantSDK
{
    [Serializable]
    public class Pin
    {
        public string content;
        public string data_request_text;
        public string data_request_url;
        public string privacy_policy_text;
        public string privacy_policy_url;
        public string terms_of_service_text;
        public string terms_of_service_url;

        public Pin()
        {
            content = "";
            data_request_text = "";
            data_request_url = "";
            privacy_policy_text = "";
            privacy_policy_url = "";
            terms_of_service_text = "";
            terms_of_service_url = "";
        }
    }
}