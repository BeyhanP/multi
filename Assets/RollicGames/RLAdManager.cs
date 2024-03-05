using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    public class RLAdManager : MonoBehaviour
    {
        public static event Action OnRollicAdsSdkInitializedEvent;

        public static event Action<string> OnRollicAdsRewardedVideoLoadedEvent;
        public static event Action<string, string> OnRollicAdsRewardedVideoFailedEvent;
        public static event Action OnRollicAdsRewardedVideoShownEvent;
        public static event Action OnRollicAdsRewardedVideoClickedEvent;
        public static event Action OnRollicAdsRewardedVideoFailedToPlayEvent;
        public static event Action<string> OnRollicAdsRewardedVideoReceivedRewardEvent;
        public static event Action OnRollicAdsRewardedVideoClosedEvent;
        public static event Action<string> OnRollicAdsRewardedVideoLeavingApplicationEvent;
        
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

        private const string LogKey = "InterstitialDisplayManager";
        private const string InterstitialEventPrefix = "InterstitialEvent";
        
        private static RLAdManager instance = null;
        private bool _isRewardAvailable = false;
        protected bool _isMediationInitialized;
        protected long _mediationInitializeTime;
        private bool _isBannerAutoShowEnabled = true;
        private bool _hasInitMediationStarted = false;

        public Action<RLRewardedAdResult> rewardedAdResultCallback { get; set; }
        public Action onInterstitialAdClosedEvent;
        public Action onInterstitialAdOpenedEvent;

        private string _appKey;
        private string _interstitialAdUnit;
        private string _rewardedVideoAdUnit;
        private string _bannerAdUnit;
        private bool _isDebugEnabled;
        protected string _bannerBackgroundColor = "";
        protected bool _isAdaptiveBannerEnabled;

        private bool _isInterstitialReady = false;

        public int bannerRequestTimerIndex = 0;
        private int interstitialRequestTimerIndex = 0;
        private int rewardedRequestTimerIndex = 0;
        public List<int> timers;
        private List<string> defaultTimerList = new List<string> { "2", "4", "8", "16" };
        
        private bool _isLevelReady;
        private bool _isTimerReady;

        private int _interstitialDisplayInterval;

        public float _lastTimeAdDisplayed;
        private int _addedValue;
        private int _timeToNextInterstitialAfterAddition;
        private float _timeSinceLastTimeAdDisplayed;
        
        private int _firstLevelToDisplay;
        private int _firstInterDisplayTimeAfterStart;
        private int _levelFrequency;
        private int _lastLevelAdDisplayed;
        private string _interShowLogic;
        private int _firstBannerDelay;
        private int _firstInterstitialDelay;
        private Timer _bannerTimer;
        private int _bannerRemainingTime;
        private Timer _intTimer;
        private int _intRemainingTime;
        private bool _isIntLocked;
        
        private static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public const string ShowLogicLevelBased = "level_based";
        public const string ShowLogicIncremental = "incremental";

        public static RLAdManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RLAdManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(RLAdManager).Name;
                        instance = obj.AddComponent<RLAdManager>();
                    }
                }

                return instance;
            }
        }
        
        public static void ExecuteOnMainThread(Action action)
        {
            if (action == null) return;

            actions.Enqueue(action);
        }

        //Reflection method
        public void StartAdManager()
        {
            if (!RemoteConfig.GetInstance().GetBool("gamekit_ads_enabled", true))
                return;
            
            var adInstance = Instance;
        }

        void Awake()
        {
            ElephantCore.OnStartGame += OnStartGame;

            if (instance == null)
            {
                instance = this as RLAdManager;
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
        
        private void Update()
        {
            while (actions.TryDequeue(out var action))
            {
                action.Invoke();
            }
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

        private void OnStartGame()
        {
            
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

            init();
            this._isMediationInitialized = false;

            MaxSdk.SetSdkKey(_appKey);
            MaxSdk.SetVerboseLogging(_isDebugEnabled);

            MaxSdk.InitializeSdk();

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedVideoLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedVideoFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedVideoShownEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedVideoClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedVideoClosedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedVideoFailedToPlayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
            
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent +=
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Fullscreen");
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent +=
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Rewarded Video");
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent +=
                (adUnitId, adInfo) => OnImpressionTrackedEvent(adUnitId, adInfo, "Banner");

            ElephantComplianceManager.GetInstance(ElephantCore.Instance.GetOpenResponse()).OnCCPAStateChangeEvent +=
                OnOnCCPAStateChangeEvent;
            ElephantComplianceManager.GetInstance(ElephantCore.Instance.GetOpenResponse()).OnGDPRStateChangeEvent +=
                OnOnGDPRStateChangeEvent;
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
            Elephant.Event("OnSdkInitializedEvent", -1, null);
            var evnt = OnRollicAdsSdkInitializedEvent;
            evnt?.Invoke();
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
            _firstBannerDelay =
                RemoteConfig.GetInstance().GetInt("gamekit_ads_first_banner_delay", 1200);
            yield return new WaitForSecondsRealtime(1.0f);
            if (RemoteConfig.GetInstance().GetBool("gamekit_rewarded_enabled", false))
            {
                RequestRewardedAd(_rewardedVideoAdUnit);
            } 
            
            if (RemoteConfig.GetInstance().GetBool("gamekit_banner_enabled", true))
            {
                InitBanner();
            } 
            
            if (RemoteConfig.GetInstance().GetBool("gamekit_interstitial_enabled", true))
            {
                InitInterstitial();
            }
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

        #endregion

        #region RewardedEvents

        private void OnRewardedVideoLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            rewardedRequestTimerIndex = 0;

            var evnt = OnRollicAdsRewardedVideoLoadedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }

        private void OnRewardedVideoFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            StartCoroutine(RequestRewardedAgain());

            var evnt = OnRollicAdsRewardedVideoFailedEvent;
            evnt?.Invoke(adUnitId, errorInfo.Message);
        }

        private void OnRewardedVideoShownEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isRewardAvailable = false;

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

        private void OnRewardedVideoFailedToPlayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            rewardedAdResultCallback?.Invoke(RLRewardedAdResult.Failed);
            RequestRewardedAd(adUnitId);

            var evnt = OnRollicAdsRewardedVideoFailedToPlayEvent;
            evnt?.Invoke();
        }

        private void OnRewardedVideoReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward,
            MaxSdkBase.AdInfo adInfo)
        {
            _isRewardAvailable = true;

            var evnt = OnRollicAdsRewardedVideoReceivedRewardEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }

        #endregion

        #region Banner
        
        public void InitBanner()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent+= OnAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsedEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpandedEvent;

            _bannerTimer = new Timer(1000);
            _bannerRemainingTime = (int)((_firstBannerDelay - ElephantCore.Instance.timeSpend / 1000));
            if (_bannerRemainingTime > 0)
            {
                _bannerTimer.Elapsed += OnBannerTimedEvent;
                _bannerTimer.Start();
            }
            else
            {
                loadBanner();
            }
            
        }

        private void OnBannerTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (_bannerRemainingTime > 0)
            {
                _bannerRemainingTime--;
            }
            else
            {
                loadBanner();
                _bannerTimer.Stop();
            }
        }
        
        public void loadBanner(bool autoShow = true)
        {
            if (!IsMediationReady()) return;

            loadBannerAsync();

            _isBannerAutoShowEnabled = autoShow;
        }
        
        private void loadBannerAsync()
        {
            ExecuteOnMainThread(() =>
            {
                MaxSdk.CreateBanner(_bannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
                MaxSdk.SetBannerExtraParameter(_bannerAdUnit, "adaptive_banner", _isAdaptiveBannerEnabled.ToString());

                if (!string.IsNullOrEmpty(_bannerBackgroundColor))
                {
                    if (ColorUtility.TryParseHtmlString(_bannerBackgroundColor, out var color))
                    {
                        MaxSdk.SetBannerBackgroundColor(_bannerAdUnit, color);
                    }
                }
            });
            
            
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

        #region Interstitial

        public void InitInterstitial()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialShown;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissed;
            Elephant.OnLevelCompleted += OnLevelCompleted;
            _isInterstitialReady = false;

            Log("Constructed");
            _addedValue = 0;
            _interstitialDisplayInterval =
                RemoteConfig.GetInstance().GetInt("gamekit_ads_interstitial_display_interval", 30);
            
            _firstInterDisplayTimeAfterStart = 
                RemoteConfig.GetInstance().GetInt("gamekit_ads_interstitial_first_int_display_after_start", 90);
            
            _firstLevelToDisplay =
                RemoteConfig.GetInstance().GetInt("gamekit_ads_interstitial_first_level_to_display", 1);
            
            _levelFrequency =
                RemoteConfig.GetInstance().GetInt("gamekit_ads_interstitial_level_frequency", 2);
            
            _interShowLogic =
                RemoteConfig.GetInstance().Get("gamekit_ads_display_logic", "level_based");
            
            _firstInterstitialDelay =
                RemoteConfig.GetInstance().GetInt("gamekit_ads_first_interstitial_delay", 1200);
            
            
            _intTimer = new Timer(1000);
            _intRemainingTime = (int)((_firstInterstitialDelay - ElephantCore.Instance.timeSpend / 1000));
            if (_intRemainingTime > 0)
            {
                _intTimer.Elapsed += OnIntTimedEvent;
                _intTimer.Start();
                _isIntLocked = true;
            }
            else
            {
                _isIntLocked = false;
            }
            
            loadInterstitial();
            if (string.Equals(_interShowLogic, ShowLogicIncremental))
            {
                InvokeRepeating("ShowInterstitialIncremental", _firstInterDisplayTimeAfterStart, _interstitialDisplayInterval);   
            }
        }
        
        private void OnIntTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (_intRemainingTime > 0)
            {
                _intRemainingTime--;
                _isIntLocked = true;
            }
            else
            {
                _intTimer.Stop();
                _isIntLocked = false;
            }
        }
        
        private void OnLevelCompleted()
        {
            if (string.Equals(_interShowLogic, ShowLogicLevelBased))
            {
                ShowInterstitialLevelBased();
            }
        }
        
        public bool IsTimerReady(float realTimeSinceStartup)
        {
            if (_isIntLocked) return false;
            
            if (_interstitialDisplayInterval == 0)
            {
                // Timer lock disabled
                Log("TimerLock: Time lock disabled");
                return true;
            }

            if (realTimeSinceStartup < _interstitialDisplayInterval && _lastTimeAdDisplayed == 0)
            {
                // Timer unlocked for first opening
                Log("Timer unlocked for first opening");
                return true;
            }
            
            // Time past since our last interstitial display
            _timeSinceLastTimeAdDisplayed = realTimeSinceStartup - _lastTimeAdDisplayed;

            // Time to next display time 
            var timeToNextInterstitial = (int) (_interstitialDisplayInterval - _timeSinceLastTimeAdDisplayed);

            // Time to next display time after addition(if any) from rewarded ads
            _timeToNextInterstitialAfterAddition = timeToNextInterstitial + _addedValue;

            if (_timeToNextInterstitialAfterAddition > 0)
            {
                Log("TimerLock: Still in the interval: " + _timeToNextInterstitialAfterAddition);
                return false;
            }
            Log("TimerLock: UNLOCKED");

            return true;
            
        }

        private bool IsLevelReady()
        {
            if (_isIntLocked) return false;
            
            if (_firstLevelToDisplay == -1 && _levelFrequency == -1)
            {
                // Level lock disabled
                Log("LevelLock: Level lock disabled");
                return true;
            }
            
            var currentLevel = MonitoringUtils.GetInstance().GetCurrentLevel().level;
            if (_firstLevelToDisplay == -1 && _levelFrequency >= 0)
            {
                // Only Level Frequency Lock enabled
                Log("LevelLock: Only Level Frequency Lock enabled - locked");
                return currentLevel - _lastLevelAdDisplayed >= _levelFrequency;
            }

            if (_levelFrequency == -1 && _firstLevelToDisplay >= 0)
            {
                // Only FirstLevel Lock enabled
                Log("LevelLock: Only FirstLevel Lock enabled - locked");
                return currentLevel > _firstLevelToDisplay;
            }
            
            // Level Lock enabled
            if (currentLevel - _lastLevelAdDisplayed >= _levelFrequency && currentLevel > _firstLevelToDisplay)
            {
                Log("LevelLock: UNLOCKED");
                return true;
            }
            Log("LevelLock: LOCKED");
            return false;
        }
        
        public void RequestInterstitial()
        {
            MaxSdk.LoadInterstitial(_interstitialAdUnit);
        }
        
        public void loadInterstitial()
        {
            RequestInterstitial();
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

        public void ShowInterstitialLevelBased()
        {
            var isLevelReady = IsLevelReady();
            var isTimerReady = IsTimerReady(Time.realtimeSinceStartup);
            
            if (!isTimerReady || !isLevelReady)
            {
                Log("LOCKED on  ShowInterstitial");
                // No Show method called
                var notShowCalledParams = Params.New();
                notShowCalledParams.Set("is_level_ready", isLevelReady.ToString()); // string
                notShowCalledParams.Set("is_timer_ready", isTimerReady.ToString()); // string
                notShowCalledParams.Set("time_since_last_time_ad_displayed", _timeSinceLastTimeAdDisplayed); // float
                notShowCalledParams.Set("is_interstitial_ready", _isInterstitialReady ? 1 : 0); // int
                notShowCalledParams.Set("added_time_value", _addedValue); // int
                notShowCalledParams.Set("last_displayed_ad_time", _lastTimeAdDisplayed); // float
                Elephant.Event(InterstitialEventPrefix + "_NotShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel().level, notShowCalledParams);
                return;
            }
            
            if (MaxSdk.IsInterstitialReady(_interstitialAdUnit))
            {
                MaxSdk.ShowInterstitial(_interstitialAdUnit);
            } 
            Log("IS AD READY: " + _isInterstitialReady);
            Log("ShowInterstitial called");
            
            var showCalledParams = Params.New();
            showCalledParams.Set("time_since_last_time_ad_displayed", _timeSinceLastTimeAdDisplayed); // float
            showCalledParams.Set("added_time_value", _addedValue); // int
            showCalledParams.Set("last_displayed_ad_time", _lastTimeAdDisplayed); // float
            showCalledParams.Set("back_up_enabled", AdConfig.GetInstance().backup_ads_enabled ? 1 : 0); // int
            showCalledParams.Set("is_interstitial_ready", _isInterstitialReady ? 1 : 0); // int
            Elephant.Event(InterstitialEventPrefix + "_ShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel().level, showCalledParams);
        }
        
        public void ShowInterstitialIncremental()
        {
            var isTimerReady = IsTimerReady(Time.realtimeSinceStartup);
            
            if (!isTimerReady)
            {
                Log("LOCKED on  ShowInterstitial");
                // No Show method called
                var notShowCalledParams = Params.New();
                notShowCalledParams.Set("is_timer_ready", isTimerReady.ToString()); // string
                notShowCalledParams.Set("time_since_last_time_ad_displayed", _timeSinceLastTimeAdDisplayed); // float
                notShowCalledParams.Set("is_interstitial_ready", _isInterstitialReady ? 1 : 0); // int
                notShowCalledParams.Set("added_time_value", _addedValue); // int
                notShowCalledParams.Set("last_displayed_ad_time", _lastTimeAdDisplayed); // float
                Elephant.Event(InterstitialEventPrefix + "_NotShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel().level, notShowCalledParams);
                return;
            }
            
            if (MaxSdk.IsInterstitialReady(_interstitialAdUnit))
            {
                MaxSdk.ShowInterstitial(_interstitialAdUnit);
            } 
            Log("IS AD READY: " + _isInterstitialReady);
            Log("ShowInterstitial called");
            
            var showCalledParams = Params.New();
            showCalledParams.Set("time_since_last_time_ad_displayed", _timeSinceLastTimeAdDisplayed); // float
            showCalledParams.Set("added_time_value", _addedValue); // int
            showCalledParams.Set("last_displayed_ad_time", _lastTimeAdDisplayed); // float
            showCalledParams.Set("back_up_enabled", AdConfig.GetInstance().backup_ads_enabled ? 1 : 0); // int
            showCalledParams.Set("is_interstitial_ready", _isInterstitialReady ? 1 : 0); // int
            Elephant.Event(InterstitialEventPrefix + "_ShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel().level, showCalledParams);
        }
        
        private void SetAdReady(string adUnitId, bool isReady)
        {
            _isInterstitialReady = isReady;
        }

        #endregion

        #region InterstitialEvents

        private void OnInterstitialLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, true);
            
            interstitialRequestTimerIndex = 0;
            
            var evnt = OnRollicAdsInterstitialLoadedEvent;
            evnt?.Invoke(adInfo.NetworkName);
        }
        
        private void OnInterstitialFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            SetAdReady(adUnitId, false);
            
            _isInterstitialReady = false;
            StartCoroutine(RequestInterstitialAgain());
            IronSourceError error = new IronSourceError(errorInfo.MediatedNetworkErrorCode, errorInfo.Message);
            var evnt = OnRollicAdsInterstitialFailedEvent;
            evnt?.Invoke(error);
        }
        
        private void OnInterstitialShown(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, false);
            _lastTimeAdDisplayed = Time.realtimeSinceStartup;
            _lastLevelAdDisplayed = MonitoringUtils.GetInstance().GetCurrentLevel().level;
            _addedValue = 0;
            
            if (onInterstitialAdOpenedEvent != null)
            {
                onInterstitialAdOpenedEvent();
            }

            RequestInterstitial();

            var evnt = OnRollicAdsInterstitialShownEvent;
            evnt?.Invoke();
        }
        
        private void OnInterstitialDismissed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, false);
            
            RequestInterstitial();
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
            SetAdReady(adUnitId, false);
            
            var ilrd = new Ilrd(adInfo, "Fullscreen");
            Elephant.AdEventV2("OnInterstitialClickedEvent", JsonUtility.ToJson(ilrd));
            var evnt = OnRollicAdsInterstitialClickedEvent;
            evnt?.Invoke();
        }

        #endregion

        protected bool IsMediationReady()
        {
            if (_isMediationInitialized)
            {
                return true;
            }

            Debug.LogError(
                "RLAdvertisementManager is not initialized properly! Please make sure that you registered OnSdkInitializedEvent event");
            return false;
        }
        
        private void Log(string message)
        {
            Debug.Log(LogKey + ": " + message);
        }
    }
}