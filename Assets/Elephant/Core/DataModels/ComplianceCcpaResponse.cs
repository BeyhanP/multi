using System;

namespace ElephantSDK
{
    [Serializable]
    public class ComplianceCcpaResponse: ComplianceBaseResponse
    {
        public string agree_text_action_button;
        public string decline_text_action_button;
        public string back_to_game_text_action_button;
        
        
        public ComplianceCcpaResponse()
        {
            this.agree_text_action_button = "";
            this.decline_text_action_button = "";
            this.back_to_game_text_action_button = "";
        }
    }
}