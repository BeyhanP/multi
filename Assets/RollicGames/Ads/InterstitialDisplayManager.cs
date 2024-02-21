using ElephantSDK;
using UnityEngine;

namespace RollicGames.Advertisements.Ads
{
    public class InterstitialDisplayManager
    {
        private const string LogKey = "InterstitialDisplayMiyavger";
        private static InterstitialDisplayManager _instance;
        private const string InterstitialEventPrefix = "InterstitialEvent";
        
        private bool _isInterstitialReady;
        private bool _isLevelReady;
        private bool _isTimerReady;

        private int _interstitialDisplayInterval;

        public float _lastTimeAdDisplayed;
        private int _addedValue;
        private int _timeToNextInterstitialAfterAddition;
        private float _timeSinceLastTimeAdDisplayed;
        
        private int _firstLevelToDisplay;
        private int _levelFrequency;
        private int _lastLevelAdDisplayed;

        private string _interstitialAdUnit;
        
        public static InterstitialDisplayManager GetInstance()
        {
            return _instance ?? (_instance = new InterstitialDisplayManager());
        }

        private InterstitialDisplayManager()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialShown;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissed;
            
            _isInterstitialReady = false;

            Log("Constructed");
        }

        public void Init(string interstitialAdUnit)
        {
            _interstitialAdUnit = interstitialAdUnit;
            _addedValue = 0;
            
            Log("Init");
        }

        public void SetRules(int interstitialDisplayInterval, int firstLevelToDisplay, int levelFrequency)
        {
            _interstitialDisplayInterval = interstitialDisplayInterval;
            _firstLevelToDisplay = firstLevelToDisplay;
            _levelFrequency = levelFrequency;
        }
        
        public void AdjustInterstitialDisplayTimer(float realTimeSinceStartup, int addedValue)
        {
            switch (addedValue)
            {
                case -1:
                    _addedValue = 0;
                    break;
                case 0:
                    // added value = 0  resets interstitial timer.
                    // To reset interstitial timer, add timePassed from last interstitial to timeLeft to next one
                    // So added value will be "time since last interstitial displayed"
                    _addedValue = (int) (realTimeSinceStartup - _lastTimeAdDisplayed);
                    
                    // Cap addedValue with interstitial display time interval
                    if (_addedValue > _interstitialDisplayInterval)
                    {
                        _addedValue = _interstitialDisplayInterval;
                    }
                    break;
                default:
                    _addedValue += addedValue;
                    break;
            }
            
            Log("Added value: " + _addedValue);
        }

        public bool IsTimerReady(float realTimeSinceStartup)
        {

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
            if (_firstLevelToDisplay == -1 && _levelFrequency == -1)
            {
                // Level lock disabled
                Log("LevelLock: Level lock disabled");
                return true;
            }
            
            var currentLevel = MonitoringUtils.GetInstance().GetCurrentLevel();
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

        public void ShowInterstitial()
        {
            var isTimerReady = IsTimerReady(Time.realtimeSinceStartup);
            var isLevelReady = IsLevelReady();
            
            if (!isTimerReady || !isLevelReady)
            {
                Log("LOCKED on  ShowInterstitial");
                // No Show method called
                var notShowCalledParams = Params.New();
                notShowCalledParams.Set("is_timer_ready", isTimerReady.ToString()); // string
                notShowCalledParams.Set("is_level_ready", isLevelReady.ToString()); // string
                notShowCalledParams.Set("time_since_last_time_ad_displayed", _timeSinceLastTimeAdDisplayed); // float
                notShowCalledParams.Set("is_interstitial_ready", _isInterstitialReady ? 1 : 0); // int
                notShowCalledParams.Set("added_time_value", _addedValue); // int
                notShowCalledParams.Set("last_displayed_ad_time", _lastTimeAdDisplayed); // float
                notShowCalledParams.CustomString(JsonUtility.ToJson(AdConfig.GetInstance().interstitial_ad_logic));
                Elephant.Event(InterstitialEventPrefix + "_NotShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel(), notShowCalledParams);
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
            showCalledParams.CustomString(JsonUtility.ToJson(AdConfig.GetInstance().interstitial_ad_logic));
            Elephant.Event(InterstitialEventPrefix + "_ShowCalled", MonitoringUtils.GetInstance().GetCurrentLevel(), showCalledParams);
        }

        
        private void SetAdReady(string adUnitId, bool isReady)
        {
            _isInterstitialReady = isReady;
        } 

        private void OnInterstitialShown(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, false);
            _lastTimeAdDisplayed = Time.realtimeSinceStartup;
            _lastLevelAdDisplayed = MonitoringUtils.GetInstance().GetCurrentLevel();
            _addedValue = 0;
            Log("OnInterstitialShown _lastTimeAdDisplayed: " + _lastTimeAdDisplayed);
            Log("OnInterstitialShown _lastLevelAdDisplayed: " + _lastLevelAdDisplayed);
        }
        
        private void OnInterstitialLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, true);
        }
        
        private void OnInterstitialFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            SetAdReady(adUnitId, false);
        }
        
        private void OnInterstitialDismissed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            SetAdReady(adUnitId, false);
        }

        private void Log(string message)
        {
            Debug.Log(LogKey + ": " + message);
        }
    }
}