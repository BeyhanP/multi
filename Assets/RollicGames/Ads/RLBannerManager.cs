using System;
using System.Collections;
using ElephantSDK;
using UnityEngine;

namespace RollicGames.Advertisements.Ads
{
    public class RLBannerManager: RLAdManager
    {
        public static event Action OnRollicAdsAdLoadedEvent;
        public static event Action<string> OnRollicAdsAdFailedEvent;
        public static event Action OnRollicAdsAdClickedEvent;
        public static event Action<string> OnRollicAdsAdExpandedEvent;
        public static event Action<string> OnRollicAdsAdCollapsedEvent;
        
        private static RLBannerManager _instance;
        private string _bannerAdUnit;
        private bool _isBannerAutoShowEnabled = true;

        public static RLBannerManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RLBannerManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(RLBannerManager).Name;
                    _instance = obj.AddComponent<RLBannerManager>();
                }
            }
            return _instance;
        }

        public void Init(string bannerAdUnit)
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent+= OnAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsedEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpandedEvent;
            
            _bannerAdUnit = bannerAdUnit;
            loadBanner();
        }
        
        public void loadBanner(bool autoShow = true)
        {
            if (!IsMediationReady()) return;

            Elephant.AdEvent("Rollic_loadBanner");
            StartCoroutine(loadBannerAsync());

            _isBannerAutoShowEnabled = autoShow;
        }
        
        private IEnumerator loadBannerAsync()
        {
            while (!this._isMediationInitialized)
                yield return null;

            long now = Utils.Timestamp();

            if ((now - this._mediationInitializeTime) <= 2000)
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
        
        
        private void showBanner()
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
        
        private void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            bannerRequestTimerIndex = 0;
            
            Elephant.AdEvent("OnAdLoadedEvent", adUnitId);
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
            
            Elephant.AdEvent("OnAdFailedEvent", adUnitId, errorInfo.Message);
            var evnt = OnRollicAdsAdFailedEvent;
            evnt?.Invoke(errorInfo.Message);
        }
        
        private void OnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Elephant.AdEvent("OnAdClickedEvent", adUnitId);
            var evnt = OnRollicAdsAdClickedEvent;
            evnt?.Invoke();
        }
        
        private void OnAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Elephant.AdEvent("OnAdCollapsedEvent", adUnitId);
            var evnt = OnRollicAdsAdCollapsedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }

        private void OnAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Elephant.AdEvent("OnAdExpandedEvent", adUnitId);
            var evnt = OnRollicAdsAdExpandedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }
    }
}