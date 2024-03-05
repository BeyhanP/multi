using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using com.adjust.sdk;
using ElephantSDK;
using RollicGames.Advertisements.Model;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif


namespace RollicGames.Advertisements
{
    /**
     * DO NOT MODIFY THIS FILE!
     */
    public class RLAdvertisementManager : MonoBehaviour
    {
        public static event Action OnRollicAdsSdkInitializedEvent;
        public static event Action OnRollicAdsAdLoadedEvent;
        public static event Action<string> OnRollicAdsAdFailedEvent;
        public static event Action OnRollicAdsAdClickedEvent;
        public static event Action<string> OnRollicAdsAdExpandedEvent;
        public static event Action<string> OnRollicAdsAdCollapsedEvent;
        public static event Action<string> OnRollicAdsInterstitialLoadedEvent;
        public static event Action<IronSourceError> OnRollicAdsInterstitialFailedEvent;
        public static event Action OnRollicAdsInterstitialDismissedEvent;
        public static event Action<string> OnRollicAdsInterstitialExpiredEvent;
        public static event Action OnRollicAdsInterstitialShownEvent;
        public static event Action OnRollicAdsInterstitialClickedEvent;
        public static event Action<string> OnRollicAdsRewardedVideoLoadedEvent;
        public static event Action<string, string> OnRollicAdsRewardedVideoFailedEvent;
        public static event Action OnRollicAdsRewardedVideoShownEvent;
        public static event Action OnRollicAdsRewardedVideoClickedEvent;
        public static event Action OnRollicAdsRewardedVideoFailedToPlayEvent;
        public static event Action<string> OnRollicAdsRewardedVideoReceivedRewardEvent;
        public static event Action OnRollicAdsRewardedVideoClosedEvent;
        public static event Action<string> OnRollicAdsRewardedVideoLeavingApplicationEvent;
        public static event Action OnFirebaseInitialized;


        private static RLAdvertisementManager instance = null;
        private bool _isRewardAvailable = false;
        private bool _isMediationInitialized;
        private long _mediationInitializeTime;
        private bool _hasInitMediationStarted = false;

        public Action<RLRewardedAdResult> rewardedAdResultCallback { get; set; }
        public Action onInterstitialAdClosedEvent;
        public Action onInterstitialAdOpenedEvent;

        private string _appKey;
        private string _interstitialAdUnit;
        private string _rewardedVideoAdUnit;
        private string _bannerAdUnit;
        private bool _isDebugEnabled;
        private string _bannerBackgroundColor = "";
        private bool _isAdaptiveBannerEnabled;

        private bool _isInterstitialReady = false;
        private bool _isBannerAutoShowEnabled = true;
        
        private int bannerRequestTimerIndex = 0;
        private int interstitialRequestTimerIndex = 0;
        private int rewardedRequestTimerIndex = 0;
        private List<int> timers;
        private List<string> defaultTimerList = new List<string>{"2","4","8","16"};
        
        public static RLAdvertisementManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RLAdvertisementManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(RLAdvertisementManager).Name;
                        instance = obj.AddComponent<RLAdvertisementManager>();
                    }
                }

                return instance;
            }
        }
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this as RLAdvertisementManager;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
#if !UNITY_EDITOR && UNITY_ANDROID 
            RollicAdsAndroid.Init();
#endif
        }

        public void init(string appKey = "", bool isDebugEnabled = false)
        {
            _appKey = RollicApplovinIDs.AppKey;
            _isDebugEnabled = isDebugEnabled;
            
#if UNITY_IOS
            _bannerAdUnit = RollicApplovinIDs.BannerAdUnitIos;
            _interstitialAdUnit = RollicApplovinIDs.InterstitialAdUnitIos;
            _rewardedVideoAdUnit = RollicApplovinIDs.RewardedAdUnitIos;
#elif UNITY_ANDROID || UNITY_EDITOR
            _bannerAdUnit = RollicApplovinIDs.BannerAdUnitAndroid;
            _interstitialAdUnit = RollicApplovinIDs.InterstitialAdUnitAndroid;
            _rewardedVideoAdUnit = RollicApplovinIDs.RewardedAdUnitAndroid;
#endif

            var timerStringList = AdConfig.GetInstance().GetList("retry_periods", defaultTimerList);
            
            timers = timerStringList
                .Select(s => Int32.TryParse(s, out int n) ? n : 0)
                .ToList();
        }

        void Start()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            InitMediation("consent_disabled");
#elif UNITY_IOS
            if (!InternalConfig.GetInstance().idfa_consent_enabled)
            {
                InitMediation("consent_disabled");
            } else {
                StartCoroutine(CheckIdfaStatus());
            }
#endif
        }
        
        private IEnumerator CheckIdfaStatus()
        {
            while (IdfaConsentResult.GetInstance().GetStatus() == IdfaConsentResult.Status.Waiting)
            {
                yield return null;
            }
            
            InitMediation(IdfaConsentResult.GetInstance().GetIdfaResultValue());
        }

        private void InitMediation(string message)
        {
            if (_hasInitMediationStarted) return;
            _hasInitMediationStarted = true;
#if UNITY_IOS && !UNITY_EDITOR
            if (message.Equals("consent_disabled"))
            {
                Elephant.Event("facebook_tracking_enabled", -1);
                RollicAdsIos.setTrackingEnabled(true);
            }
            else
            {
                if (message.Equals("Authorized"))
                {
                    Elephant.Event("facebook_tracking_enabled", -1);
                    RollicAdsIos.setTrackingEnabled(true);
                }
                else
                {
                    Elephant.Event("facebook_tracking_disabled", -1);
                    RollicAdsIos.setTrackingEnabled(false);
                }
            }
#endif
            
            this._isMediationInitialized = false;
            
            MaxSdk.SetSdkKey(_appKey);
            MaxSdk.SetVerboseLogging(_isDebugEnabled);
            
            MaxSdk.InitializeSdk();
            
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialShownEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedVideoLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedVideoFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedVideoShownEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedVideoClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedVideoClosedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedVideoFailedToPlayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent+= OnAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsedEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpandedEvent;

            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += 
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Fullscreen");
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += 
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Rewarded Video");
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent +=
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Banner");
            
            ElephantComplianceManager.GetInstance(ElephantCore.Instance.GetOpenResponse()).OnCCPAStateChangeEvent += OnOnCCPAStateChangeEvent;
            ElephantComplianceManager.GetInstance(ElephantCore.Instance.GetOpenResponse()).OnGDPRStateChangeEvent += OnOnGDPRStateChangeEvent;
        }

        #region AdEvents

        #region InitEvents

        private void OnSdkInitializedEvent(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            StartCoroutine(LoadAdsAfterInitialization());
            this._isMediationInitialized = true;
            this._mediationInitializeTime = Utils.Timestamp();

            StartCoroutine(SendSdkInitEvents(sdkConfiguration));
        }

        private IEnumerator SendSdkInitEvents(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            if (ElephantCore.Instance != null)
            {
                ElephantCore.Instance.adjustId = string.IsNullOrEmpty(Adjust.getAdid()) ? "" : Adjust.getAdid();
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
            Elephant.Event("OnSdkInitializedEvent", -1,  null);
            var evnt = OnRollicAdsSdkInitializedEvent;
            evnt?.Invoke();
        }

        #endregion

        #region InterstitialEvents

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isInterstitialReady = true;
            interstitialRequestTimerIndex = 0;


            var evnt = OnRollicAdsInterstitialLoadedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }
        
        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _isInterstitialReady = false;
            StartCoroutine(RequestInterstitialAgain());
            IronSourceError error = new IronSourceError(errorInfo.MediatedNetworkErrorCode, errorInfo.Message);
            var evnt = OnRollicAdsInterstitialFailedEvent;
            evnt?.Invoke(error);
        }
        
        private void OnInterstitialShownEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isInterstitialReady = false;
            if (onInterstitialAdOpenedEvent != null)
            {
                onInterstitialAdOpenedEvent();
            }

            RequestInterstitial(_interstitialAdUnit);

            var evnt = OnRollicAdsInterstitialShownEvent;
            evnt?.Invoke();
        }
        
        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isInterstitialReady = false;
            RequestInterstitial(_interstitialAdUnit);
            if (onInterstitialAdClosedEvent != null)
            {
                onInterstitialAdClosedEvent();
            }

            var ilrd = new Ilrd(adInfo, "Fullscreen");
            Elephant.AdEventV2("OnInterstitialDismissedEvent", JsonUtility.ToJson(ilrd));
            var evnt = OnRollicAdsInterstitialDismissedEvent;
            evnt?.Invoke();
        }
        
        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var ilrd = new Ilrd(adInfo, "Fullscreen");
            Elephant.AdEventV2("OnInterstitialClickedEvent", JsonUtility.ToJson(ilrd));
            var evnt = OnRollicAdsInterstitialClickedEvent;
            evnt?.Invoke();
        }

        #endregion

        #region RewardedEvents

        private void OnRewardedVideoLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            ChangeButtonAvailability(true);
            rewardedRequestTimerIndex = 0;

            var evnt = OnRollicAdsRewardedVideoLoadedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }
        
        private void OnRewardedVideoFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            ChangeButtonAvailability(false);
            StartCoroutine(RequestRewardedAgain());

            var evnt = OnRollicAdsRewardedVideoFailedEvent;
            evnt?.Invoke(adUnitId, errorInfo.Message);
        }
        
        private void OnRewardedVideoShownEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isRewardAvailable = false;
            ChangeButtonAvailability(false);
         
            var evnt = OnRollicAdsRewardedVideoShownEvent;
            evnt?.Invoke();
        }
        
        private void OnRewardedVideoClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var evnt = OnRollicAdsRewardedVideoClickedEvent;
            evnt?.Invoke();
        }
        
        private void OnRewardedVideoClosedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            RequestRewardedAd(adUnitId);
            CheckReward();
            
            var evnt = OnRollicAdsRewardedVideoClosedEvent;
            evnt?.Invoke();
        }
        
        private void OnRewardedVideoFailedToPlayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            ChangeButtonAvailability(false);
            rewardedAdResultCallback?.Invoke(RLRewardedAdResult.Failed);
            RequestRewardedAd(adUnitId);
            
            var evnt = OnRollicAdsRewardedVideoFailedToPlayEvent;
            evnt?.Invoke();
        }
        
        private void OnRewardedVideoReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _isRewardAvailable = true;
            
            var evnt = OnRollicAdsRewardedVideoReceivedRewardEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }

        #endregion

        #region BannerEvents

        private void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            bannerRequestTimerIndex = 0;
            
            var evnt = OnRollicAdsAdLoadedEvent;
            evnt?.Invoke();

            if (_isBannerAutoShowEnabled)
            {
                showBanner();
            }
        }
        
        private void OnAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            StartCoroutine(RequestBannerAgain());
            
            var evnt = OnRollicAdsAdFailedEvent;
            evnt?.Invoke(errorInfo.Message);
        }
        
        private void OnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var evnt = OnRollicAdsAdClickedEvent;
            evnt?.Invoke();
        }
        
        private void OnAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var evnt = OnRollicAdsAdCollapsedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }

        private void OnAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var evnt = OnRollicAdsAdExpandedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }
        
        #endregion
        
        #region ILRDEvents

        private void OnImpressionTrackedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo, string adFormat)
        {
            var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            adRevenue.setRevenue(adInfo.Revenue, "USD");
            adRevenue.setAdRevenueNetwork(adInfo.NetworkName);
            adRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
            adRevenue.setAdRevenuePlacement(adInfo.Placement);
            adRevenue.addCallbackParameter("ad_format", adFormat);
            adRevenue.addCallbackParameter("network_placement", adInfo.NetworkPlacement);
            adRevenue.addCallbackParameter("creative_id", adInfo.CreativeIdentifier);

            Adjust.trackAdRevenue(adRevenue);
        }

        #endregion
        
        #endregion
        
        #region UserConsentEvents

        private void OnOnCCPAStateChangeEvent(bool accepted)
        {
            MaxSdk.SetDoNotSell(!accepted);
        }
        
        private void OnOnGDPRStateChangeEvent(bool accepted)
        {
            MaxSdk.SetHasUserConsent(accepted);
        }

        #endregion

        IEnumerator LoadAdsAfterInitialization()
        {
            yield return new WaitForSecondsRealtime(3.0f);
            RequestInterstitial(_interstitialAdUnit);
            yield return new WaitForSecondsRealtime(1.0f);
            RequestRewardedAd(_rewardedVideoAdUnit);
            
        }

        #region Rewarded

        private void RequestRewardedAd(string adUnitId)
        {
            MaxSdk.LoadRewardedAd(adUnitId);
        }

        public bool isRewardedVideoAvailable()
        {
            if (!IsMediationReady()) return false;
            
            return MaxSdk.IsRewardedAdReady(_rewardedVideoAdUnit);
        }

        public void showRewardedVideo()
        {
            if (!IsMediationReady()) return;
            
            MaxSdk.ShowRewardedAd(_rewardedVideoAdUnit);
        }


        #endregion

        #region Banner

        public void loadBanner(bool autoShow = true)
        {
            if (!IsMediationReady()) return;
            
            StartCoroutine(loadBannerAsync());

            _isBannerAutoShowEnabled = autoShow;
        }
        
        private IEnumerator loadBannerAsync()
        {
            while (!this._isMediationInitialized)
                yield return null;

            long now = Utils.Timestamp();
            
            if ((now - this._mediationInitializeTime) <= 2000 )
            {
                yield return new WaitForSecondsRealtime(2.0f);
            }
            
            MaxSdk.CreateBanner(_bannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(_bannerAdUnit, "adaptive_banner", _isAdaptiveBannerEnabled.ToString());

            if (!string.IsNullOrEmpty(_bannerBackgroundColor))
            {
                if (ColorUtility.TryParseHtmlString(_bannerBackgroundColor, out var color))
                {
                    MaxSdk.SetBannerBackgroundColor(_bannerAdUnit, color);
                }
            }
        }
        
        public void showBanner()
        {
            MaxSdk.ShowBanner(_bannerAdUnit);
        }

        public void hideBanner()
        {
            MaxSdk.HideBanner(_bannerAdUnit);
        }

        public void destroyBanner()
        {
            MaxSdk.DestroyBanner(_bannerAdUnit);
        }

        public void SetBannerBackground(string backgroundColor)
        {
            _bannerBackgroundColor = backgroundColor;
        }
        
        public void SetAdaptiveBannerEnabled(bool isAdaptiveBannerEnabled)
        {
            _isAdaptiveBannerEnabled = isAdaptiveBannerEnabled;
        }

        public float GetBannerHeight()
        {
            if (!IsMediationReady())
            {
                return -1;
            }
            
#if UNITY_IOS
            return RollicAdsIos.getPixelValue(MaxSdkUtils.GetAdaptiveBannerHeight());
#elif UNITY_ANDROID
            return RollicAdsAndroid.ConvertDpToPixel(MaxSdkUtils.GetAdaptiveBannerHeight());
#else
            return MaxSdkUtils.GetAdaptiveBannerHeight();
#endif
        }

        #endregion

        #region Interstitial

        private void RequestInterstitial(string adUnitId)
        {
            MaxSdk.LoadInterstitial(adUnitId);
        }
        
        public bool isInterstitialReady()
        {
            return _isInterstitialReady;
        }

        public void loadInterstitial()
        {
            RequestInterstitial(_interstitialAdUnit);
        }

        public void showInterstitial()
        {
            MaxSdk.ShowInterstitial(_interstitialAdUnit);
        }

        #endregion

        void ChangeButtonAvailability(bool isAvailable)
        {
        }

        IEnumerator RequestInterstitialAgain()
        {
            if (timers == null) throw new Exception("RLAdvertisementManager has not been initialized!");
            
            yield return new WaitForSecondsRealtime(timers[interstitialRequestTimerIndex]);
            if (interstitialRequestTimerIndex < timers.Count - 1)
            {
                interstitialRequestTimerIndex++;    
            }
            else
            {
                interstitialRequestTimerIndex = 0;
            }

            loadInterstitial();
        }

        IEnumerator RequestRewardedAgain()
        {
            if (timers == null) throw new Exception("RLAdvertisementManager has not been initialized!");
            
            yield return new WaitForSecondsRealtime(timers[rewardedRequestTimerIndex]);
            if (rewardedRequestTimerIndex < timers.Count - 1)
            {
                rewardedRequestTimerIndex++;    
            }
            else
            {
                rewardedRequestTimerIndex = 0;
            }

            RequestRewardedAd(_rewardedVideoAdUnit);
        }

        IEnumerator RequestBannerAgain()
        {
            if (timers == null) throw new Exception("RLAdvertisementManager has not been initialized!");
            
            yield return new WaitForSecondsRealtime(timers[bannerRequestTimerIndex]);
            if (bannerRequestTimerIndex < timers.Count - 1)
            {
                bannerRequestTimerIndex++;    
            }
            else
            {
                bannerRequestTimerIndex = 0;
            }

            loadBanner();
        }

        void CheckReward()
        {
            
            if (_isRewardAvailable)
            {
                rewardedAdResultCallback(RLRewardedAdResult.Finished);
            }
            else
            {
                rewardedAdResultCallback(RLRewardedAdResult.Skipped);
            }
        }

        private bool IsMediationReady()
        {
            if (_isMediationInitialized)
            {
                return true;
            }

            Debug.LogError("RLAdvertisementManager is not initialized properly! Please make sure that you registered OnSdkInitializedEvent event");
            return false;
        }
    }
}