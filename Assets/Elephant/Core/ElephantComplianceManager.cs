using System;
using Facebook.Unity;
using UnityEngine;

namespace ElephantSDK
{
    public class ElephantComplianceManager
    {
        private static ElephantComplianceManager instance;

        private OpenResponse _openResponse;

        public event Action<bool> OnCCPAStateChangeEvent;
        public event Action<bool> OnGDPRStateChangeEvent; 

        public static ElephantComplianceManager GetInstance(OpenResponse openResponse)
        {
            return instance ?? (instance = new ElephantComplianceManager(openResponse));
        }

        private ElephantComplianceManager(OpenResponse openResponse)
        {
            _openResponse = openResponse;
        }

        public void UpdateOpenResponse(OpenResponse openResponse)
        {
            this._openResponse = openResponse;
        }


        #region Compliance Display

        public void ShowTosAndPp(OnOpenResult onOpen)
        {
            if (_openResponse.compliance.tos == null) return;
            
            var tos = _openResponse.compliance.tos;
            if (tos.required)
            {
                onOpen(true, tos);
            }
            else
            {
                ElephantCore.Instance.OpenIdfaConsent();
                onOpen(false, tos);
            }
        }

        public void ShowCcpa()
        {
            if (_openResponse.compliance.ccpa == null) return;

            if (!_openResponse.compliance.ccpa.required) return;
            
            var ccpa = _openResponse.compliance.ccpa;
#if UNITY_EDITOR
// no-op
            ElephantLog.Log("COMPLIANCE","ShowCcpa Content");
#elif UNITY_ANDROID
            ElephantAndroid.ShowCcpaDialog("CCPA", ccpa.title, ccpa.content, ccpa.privacy_policy_text, 
                ccpa.privacy_policy_url, ccpa.decline_text_action_button, ccpa.agree_text_action_button, 
                ccpa.back_to_game_text_action_button);
#elif UNITY_IOS
            ElephantIOS.showCcpaPopUpView("CCPA", ccpa.title, ccpa.content, ccpa.privacy_policy_text, 
                ccpa.privacy_policy_url, ccpa.decline_text_action_button, ccpa.agree_text_action_button, 
                ccpa.back_to_game_text_action_button);
#endif
        }

        public void ShowGdprAdConsent()
        {
            if (_openResponse.compliance.gdpr_ad_consent == null) return;

            if (!_openResponse.compliance.gdpr_ad_consent.required) return;
            
            var gdpr = _openResponse.compliance.gdpr_ad_consent;
#if UNITY_EDITOR
// no-op
            ElephantLog.Log("COMPLIANCE","ShowGdprAdConsent Content");    
#elif UNITY_ANDROID
            ElephantAndroid.ShowCcpaDialog("GDPR_AD_CONSENT", gdpr.title, gdpr.content, gdpr.privacy_policy_text, 
                gdpr.privacy_policy_url, gdpr.decline_text_action_button, gdpr.agree_text_action_button, 
                gdpr.back_to_game_text_action_button);
#elif UNITY_IOS
            ElephantIOS.showCcpaPopUpView("GDPR_AD_CONSENT", gdpr.title, gdpr.content, gdpr.privacy_policy_text, 
                gdpr.privacy_policy_url, gdpr.decline_text_action_button, gdpr.agree_text_action_button, 
                gdpr.back_to_game_text_action_button);
#endif
        }

        public void ShowBlockedPopUp()
        {
            if (_openResponse.compliance.blocked == null) return;

            if (!_openResponse.compliance.blocked.is_blocked) return;
            
            var blocked = _openResponse.compliance.blocked;
            
#if UNITY_EDITOR
            // No-op
#elif UNITY_IOS
            ElephantIOS.showBlockedPopUpView(blocked.title, blocked.content, blocked.warning_text, blocked.button_text);
#elif UNITY_ANDROID
            ElephantAndroid.showBlockedDialog(blocked.title, blocked.content, blocked.warning_text, blocked.button_text);
#endif
        }
        
        public bool CheckForceUpdate()
        {
            if (!IsForceUpdateNeeded()) return false;
            
            var forceUpdateEventParams = Params.New()
                .Set("version_seen", Application.version);

            Elephant.Event("force_update_seen", -1, forceUpdateEventParams);

#if UNITY_EDITOR
            // no-op
#elif UNITY_ANDROID
                ElephantAndroid.showForceUpdate("Update needed", "Please update your application");
#elif UNITY_IOS
                ElephantIOS.showForceUpdate("Update needed", "Please update your application");
#else
                // no-op
#endif

            return true;

        }
        
        private bool IsForceUpdateNeeded()
        {
            var internalConfig = _openResponse?.internal_config;
            if (internalConfig == null) return false;

            if (string.IsNullOrEmpty(internalConfig.min_app_version)) return false;

            return VersionCheckUtils.GetInstance()
                .CompareVersions(Application.version, internalConfig.min_app_version) < 0;
            
        }

        #endregion

        #region Compliance Results

        public void SendTosAccept()
        {
            var data = new ComplianceRequestData();
            data.FillBaseData(ElephantCore.Instance.GetCurrentSession().session_id);
            var request = new ElephantRequest(ElephantCore.TOS_ACCEPT_EP, data);
            
            ElephantCore.Instance.AddToQueue(request);
            
            ElephantUI.Instance.PlayGame();
            ElephantCore.Instance.OpenIdfaConsent();

            if (FB.IsInitialized)
            {
                FB.Mobile.SetAdvertiserIDCollectionEnabled(true);
                FB.Mobile.SetAdvertiserTrackingEnabled(true);
            }

#if UNITY_ANDROID
            ShowCcpa();
            ShowGdprAdConsent();
#endif 
        }
        
        public void SendCcpaStatus(bool accepted)
        {
            var data = CcpaGdprStatusRequestData.CreateCcpaGdprStatusRequestData(accepted);
            var request = new ElephantRequest(ElephantCore.CCPA_STATUS, data);
            
            ElephantCore.Instance.AddToQueue(request);
            instance.OnCCPAStateChangeEvent(accepted);
        }
        
        public void SendGdprAdConsentStatus(bool accepted)
        {
            var data = CcpaGdprStatusRequestData.CreateCcpaGdprStatusRequestData(accepted);
            var request = new ElephantRequest(ElephantCore.GDPR_AD_CONSENT, data);
            
            ElephantCore.Instance.AddToQueue(request);
            instance.OnGDPRStateChangeEvent(accepted);
        }

        #endregion
    }
}