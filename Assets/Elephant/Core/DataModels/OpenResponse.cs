using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class Compliance
    {
        public ComplianceTosResponse tos;
        public ComplianceCcpaResponse ccpa;
        public ComplianceCcpaResponse gdpr_ad_consent;
        public ComplianceBlockedResponse blocked;

        public Compliance()
        {
            tos = new ComplianceTosResponse();
            ccpa = new ComplianceCcpaResponse();
            gdpr_ad_consent = new ComplianceCcpaResponse();
            blocked = new ComplianceBlockedResponse();
        }
    }
    
    [Serializable]
    public class OpenResponse
    {
        public string user_id;
        public string player_id;
        public ZisPlayerIdResponse zisPlayer;
        public bool consent_required;
        public bool consent_status;
        public string remote_config_json; // json
        public AdConfig ad_config;
        public InternalConfig internal_config;
        public List<MirrorData> mirror_data;
        public Compliance compliance;
        public string hash;

        public OpenResponse()
        {
            this.user_id = "";
            this.player_id = "";
            this.zisPlayer = new ZisPlayerIdResponse();
            this.consent_required = false;
            this.remote_config_json = JsonUtility.ToJson(new ConfigResponse());
            this.ad_config = AdConfig.GetInstance();
            this.internal_config = InternalConfig.GetInstance();
            this.mirror_data = new List<MirrorData>();
            this.compliance = new Compliance();
            this.hash = "";
        }
    }
}