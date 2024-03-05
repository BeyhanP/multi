using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using com.adjust.sdk;
using Facebook.Unity;
using NUlid;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.Networking;

namespace ElephantSDK
{
    public delegate void OnInitialized();
    public delegate void OnOpenResult(bool gdprRequired, ComplianceTosResponse tos);
    public delegate void OnRemoteConfigLoaded();
    public delegate void OnNotificationOpened();
    public delegate void OnOfferUIFetched();

    public class ElephantCore : MonoBehaviour
    {
        public string GameID = "";
        public string GameSecret = "";

        public static ElephantCore Instance = null;
        
        public static event Action OnStartGame;
        
        private const string ELEPHANT_BASE_URL = "https://newapi.rollic.gs/v3";
        public const string LIVEOPS_BASE_URL = "https://liveopsapi.rollic.gs/api/v2";

        public const string OPEN_EP = ELEPHANT_BASE_URL + "/open";
        public const string USER_EP = ELEPHANT_BASE_URL + "/user";
        public const string EVENT_EP = ELEPHANT_BASE_URL + "/event";
        public const string SESSION_EP = ELEPHANT_BASE_URL + "/session";
        public const string MONITORING_EP = ELEPHANT_BASE_URL + "/monitoring";
        public const string TRANSACTION_EP = ELEPHANT_BASE_URL + "/transaction";
        public const string AD_REVENUE_EP = ELEPHANT_BASE_URL + "/adrevenue";
        public const string IAP_STATUS_EP = ELEPHANT_BASE_URL + "/user/iap/status";
        public const string IAP_VERIFY_EP = ELEPHANT_BASE_URL + "/iap/verify";
        public const string PIN_EP = ELEPHANT_BASE_URL + "/gdpr/pin";
        public const string TOS_ACCEPT_EP = ELEPHANT_BASE_URL + "/tos/accept";
        public const string CCPA_STATUS = ELEPHANT_BASE_URL + "/ccpa/status";
        public const string GDPR_AD_CONSENT = ELEPHANT_BASE_URL + "/gdpr/status";
        public const string SETTINGS_EP = ELEPHANT_BASE_URL + "/settings";
        public const string OFFER_EP = LIVEOPS_BASE_URL + "/offers";
        public const string HEALTH_CHECK_EP = "https://newapi.rollic.gs/health_check";
        public const string NOTIFICATION_EP = "https://notificationapi.rollic.gs/api/v1/register";
        public const string OFFERURLS_EP = LIVEOPS_BASE_URL + "/offers/assets";


        private Queue<ElephantRequest> _queue = new Queue<ElephantRequest>();
        private List<ElephantRequest> _failedQueue = new List<ElephantRequest>();
        private bool processQueues = false;
        private bool processFailedBatch = true;

        private static string QUEUE_DATA_FILE = "ELEPHANT_DATA_QUEUE_";
        
        private bool sdkIsReady = false;
        private bool circuitBreakerEnabled = false;
        private int health_check_retry_period = 300;
        private int fail_retry_count = 4;

        private bool openRequestWaiting;
        private bool openRequestSucceded;
        private SessionData currentSession;
        internal long realSessionId;
        internal long firstInstallTime;
        internal long timeSpend;
        internal long installTime;
        internal string idfa = "";
        internal string idfv = "";
        internal string adjustId = "";
        internal string buildNumber = "";
        internal string consentStatus = "NotDetermined";
        internal string userId = "";
        internal string clientId = "";
        internal List<MirrorData> mirrorData;
        internal int eventOrder = 0;
        internal float focusLostTime = 0;
        internal string networkName = "";
        internal string campaignName = "";
        internal string adGroupName = "";
        internal string creativeName = "";
        internal double uaCost;
        internal long firstOpenTimeStamp;
        
        

        public bool isIapBanned;
        
        private OpenResponse openResponse;
        private string cachedOpenResponse;
        private ElephantComplianceManager _elephantComplianceManager;

        private static int MAX_FAILED_COUNT = 250;
        
        private static string REMOTE_CONFIG_FILE = "ELEPHANT_REMOTE_CONFIG_DATA";
        private static string FIRST_OPEN_TIME = "ELEPHANT_FIRST_OPEN_TIME";
        private static string USER_DB_ID = "USER_DB_ID";
        private static string CLIENT_DB_ID = "CLIENT_DB_ID";
        private static string CACHED_OPEN_RESPONSE = "CACHED_OPEN_RESPONSE";
        public static string OFFLINE_FLAG = "OFFLINE_FLAG";
        public static string FIRST_OPEN_TS = "FIRST_OPEN_TS";
        public static string TOS_REMINDER_SHOWN_DATA = "TOS_REMINDER_SHOWN_DATA";
        public static string TimeSpend = "Time_Spend";


        public static event OnInitialized onInitialized;
        public static event OnOpenResult onOpen;
        public static event OnRemoteConfigLoaded onRemoteConfigLoaded;
        public static event OnNotificationOpened onNotificationOpened;
        public static event OnOfferUIFetched onOfferUIFetched;
        
        
        private float[] _fpsBuffer = new float[60];
        private float _lastUpdated;
        private int _c = 0;
        private float _fps;
        
        private string deviceToken;
        
        private bool isPushEnabled = false;

        private List<Coroutine> _activeDownloads = new List<Coroutine>();

        private bool isOfferAssetsReady = false;
        private bool isOfferProductsReady = false;

        public bool isSoundFixEnabled = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR && UNITY_ANDROID
            ElephantAndroid.Init();
#endif

            ElephantLog.GetInstance(ElephantLogLevel.Prod);
        }

        void Start()
        {
            RebuildQueue();
            processQueues = true;
        }
        
        void Update()
        {
            if (!InternalConfig.GetInstance().monitoring_enabled) return;
            
            LogMonitoringData();
        }

        private void LogMonitoringData()
        {
            if (InternalConfig.GetInstance().memory_usage_enabled)
            {
#if UNITY_EDITOR
#elif UNITY_IOS
                MonitoringUtils.GetInstance().SetMemoryUsage(ElephantIOS.gameMemoryUsage());
                MonitoringUtils.GetInstance().SetMemoryUsagePercentage(ElephantIOS.gameMemoryUsagePercent());
#elif UNITY_ANDROID
                MonitoringUtils.GetInstance().SetMemoryUsage(ElephantAndroid.GameMemoryUsage());
                MonitoringUtils.GetInstance().SetMemoryUsagePercentage(ElephantAndroid.GameMemoryUsagePercentage());
#endif
            } 
            
            _fpsBuffer[_c] = 1.0f / Time.deltaTime;
            _c = (_c + 1) % _fpsBuffer.Length;
            if (Time.time - _lastUpdated >= 1)
            {
                _lastUpdated = Time.time;
                _fps = MonitoringUtils.GetInstance().CalculateFps(_fpsBuffer);
                
                if (float.IsInfinity(_fps) || float.IsNaN(_fps) ) return;
                MonitoringUtils.GetInstance().LogFps(Math.Round(_fps, 1));
                MonitoringUtils.GetInstance().LogCurrentLevel();
                
            }
        }

        public void Init()
        {
            this.GameID = ElephantThirdPartyIds.GameId;
            this.GameSecret = ElephantThirdPartyIds.GameSecret;


            if (GameID.Trim().Length == 0 || GameSecret.Trim().Length == 0)
            {
                ElephantLog.LogError("ELEPHANT INIT",
                    "Game ID and Game Secret are not present, make sure you replace them with yours using Window -> Elephant -> Edit Settings");
            }

            InvokeRepeating(nameof(CheckApiHealth), 1, health_check_retry_period);
            VersionCheckUtils.GetInstance();
            
#if UNITY_EDITOR
            buildNumber = "";
#elif UNITY_ANDROID
            buildNumber = ElephantAndroid.getBuildNumber();
#elif UNITY_IOS
            buildNumber = ElephantIOS.getBuildNumber();
#else 
            buildNumber = "";
#endif
            
#if UNITY_EDITOR
            // No-op
            firstInstallTime = 0;
#elif UNITY_IOS
            firstInstallTime = ElephantIOS.getFirstInstallTime();
#elif UNITY_ANDROID
            firstInstallTime = ElephantAndroid.GetFirstInstallTime();
#else 
            firstInstallTime = 0;
#endif
            
            if (!FB.IsInitialized)
            {
                FB.Init(ElephantThirdPartyIds.FacebookAppId, clientToken: ElephantThirdPartyIds.FacebookClientToken, onInitComplete: OnFbInitComplete);
            }
            else
            {
                FB.ActivateApp();
                FB.Mobile.SetAdvertiserIDCollectionEnabled(false);
                FB.Mobile.SetAdvertiserTrackingEnabled(false);
            }
 
#if !UNITY_EDITOR && UNITY_ANDROID
            AdjustConfig config = new AdjustConfig(ElephantThirdPartyIds.AdjustAppKey, AdjustEnvironment.Production);
            config.setAttributionChangedDelegate(OnAttrChange);
            Adjust.start(config);
#elif UNITY_IOS
            try
            {
                if (VersionCheckUtils.GetInstance()
                        .CompareVersions(Device.systemVersion, "14.5") < 0)
                {
                    AdjustConfig config = new AdjustConfig(ElephantThirdPartyIds.AdjustAppKey, AdjustEnvironment.Production);
                    config.setAttributionChangedDelegate(OnAttrChange);
                    Adjust.start(config);
                } 
            }
            catch (Exception e)
            {
                // ignored
            }
#endif
            
            

            StartCoroutine(InitSDK());
        }

        private IEnumerator InitSDK()
        {

            string savedConfig = Utils.ReadFromFile(REMOTE_CONFIG_FILE);
            userId = Utils.ReadFromFile(USER_DB_ID) ?? "";
            clientId = Utils.ReadFromFile(CLIENT_DB_ID) ?? "";
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = Ulid.NewUlid().ToString();
                Utils.SaveToFile(CLIENT_DB_ID, clientId);
            }

            var firstOpenString = Utils.ReadFromFile(FIRST_OPEN_TIME);
            if (!string.IsNullOrEmpty(firstOpenString))
            {
                var longFirstOpenTime = Convert.ToInt64(firstOpenString);
                installTime = longFirstOpenTime;
            }

            ElephantLog.Log("Init","Remote Config From File --> " + savedConfig);
            
            var isUsingRemoteConfig = 0;

            openResponse = new OpenResponse();

            if (savedConfig != null)
            {
                RemoteConfig.GetInstance().Init(savedConfig);
                RemoteConfig.GetInstance().SetFirstOpen(false);
                openResponse.remote_config_json = savedConfig;
            }
            else
            {
                // First open 
                RemoteConfig.GetInstance().SetFirstOpen(true);
                firstOpenTimeStamp = Utils.Timestamp();
                Utils.SaveToFile(FIRST_OPEN_TS, firstOpenTimeStamp.ToString());
                installTime = Utils.Timestamp();
                Utils.SaveToFile(FIRST_OPEN_TIME, installTime.ToString());
                
            }

            openResponse.user_id = userId;
            
            openRequestWaiting = true;
            openRequestSucceded = false;

            float startTime = Time.time;
            var realTimeSinceStartup = Time.realtimeSinceStartup;
            var realTimeBeforeRequest = DateTime.Now;
            var savedTimeSpend = Utils.ReadFromFile(TimeSpend);
            timeSpend = !string.IsNullOrEmpty(savedTimeSpend) ? long.Parse(savedTimeSpend) : 0;

            RequestIDFAAndOpen();

            while (openRequestWaiting && (Time.time - startTime) < 5f)
            {
                yield return null;
            }

            isUsingRemoteConfig = openRequestSucceded ? 1 : -1;

            ElephantLog.Log("OPEN REQUEST", JsonUtility.ToJson(openResponse));

            var parameters = Params.New()
                .Set("real_duration", (DateTime.Now - realTimeBeforeRequest).TotalMilliseconds)
                .Set("game_duration", (Time.time - startTime) * 1000)
                .Set("real_time_since_startup", (Time.realtimeSinceStartup - realTimeSinceStartup) * 1000)
                .Set("is_using_remote_config", isUsingRemoteConfig)
                .CustomString(JsonUtility.ToJson(openResponse));
            
            Elephant.Event("open_request", -1, parameters);

            RemoteConfig.GetInstance().Init(openResponse.remote_config_json);
            AdConfig.GetInstance().Init(openResponse.ad_config);
            Utils.SaveToFile(REMOTE_CONFIG_FILE, openResponse.remote_config_json);
            Utils.SaveToFile(USER_DB_ID, openResponse.user_id);
            Utils.SaveToFile(CACHED_OPEN_RESPONSE, JsonUtility.ToJson(openResponse));
            userId = openResponse.user_id;
            mirrorData = openResponse.mirror_data ?? new List<MirrorData>();
            currentSession.user_tag = RemoteConfig.GetInstance().GetTag();
            
            // T0 - Check Network Reachability
            if (InternalConfig.GetInstance().reachability_check_enabled)
            {
                Elephant.ShowNetworkOfflineDialog();

                while (!Utils.IsConnected())
                {
                    yield return null;
                }                
            }

            _elephantComplianceManager = ElephantComplianceManager.GetInstance(openResponse);

            isSoundFixEnabled = RemoteConfig.GetInstance().GetBool("sound_fix_enabled", false);

            // T1 - First check: Force Update
           // if (_elephantComplianceManager.CheckForceUpdate()) yield break;

            // T2 - check if the user is blocked from data deletion
            _elephantComplianceManager.ShowBlockedPopUp();
            if (openResponse.compliance.blocked.is_blocked) yield break;

            if (onOpen != null)
            {
                // T3 - show tos and pp (replacement for old gdpr)
                _elephantComplianceManager.ShowTosAndPp(onOpen);
            }
            else
            {
                ElephantLog.Log("ELEPHANT INIT","ElephantSDK onOpen event is not handled");
            }
            
            // T4 - start zynga player id request async..
            // RIP ZIS Request...
            
            // T5 - if offline session flag is filled, send the data and flush it
            var offlineFlag = Utils.ReadFromFile(OFFLINE_FLAG);
            if (!string.IsNullOrEmpty(offlineFlag))
            {
                var param = Params.New().Set("sessionId", offlineFlag);
                Elephant.Event("previous_offline_session", -1, param);
                Utils.SaveToFile(OFFLINE_FLAG, "");
            }
            
            sdkIsReady = true;
            if (onRemoteConfigLoaded != null)
                onRemoteConfigLoaded();
        }
        
        public void OpenIdfaConsent()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (InternalConfig.GetInstance().idfa_consent_enabled)
            {
                InternalConfig internalConfig = InternalConfig.GetInstance();

                Elephant.Event("ask_idfa_consent", -1);
                ElephantIOS.showIdfaConsent(internalConfig.idfa_consent_type,
                    internalConfig.idfa_consent_delay, internalConfig.idfa_consent_position,
                    internalConfig.consent_text_body, internalConfig.consent_text_action_body,
                    internalConfig.consent_text_action_button, internalConfig.terms_of_service_text,
                    internalConfig.privacy_policy_text, internalConfig.terms_of_service_url,
                    internalConfig.privacy_policy_url);
            }
#endif
        }

        private void OnAttrChange(AdjustAttribution adjustAttribution)
        {
            this.adjustId = adjustAttribution.adid;
            this.networkName = adjustAttribution.network;
            this.campaignName = adjustAttribution.campaign;
            this.adGroupName = adjustAttribution.adgroup;
            this.creativeName = adjustAttribution.creative;
            if (adjustAttribution.costAmount != null)
            {
                this.uaCost = (double)adjustAttribution.costAmount;
            }
            
            ElephantLog.Log("Adjust attr",adjustAttribution.adid);
            ElephantLog.Log("Adjust attr",adjustAttribution.network);
        }

        private void SendVersionsEvent()
        {
            var versionCheckUtils = VersionCheckUtils.GetInstance();
            var versionData = new VersionData(Application.version, ElephantVersion.SDK_VERSION,
                SystemInfo.operatingSystem,  versionCheckUtils.UnityVersion, versionCheckUtils.GameKitVersion);

            var parameters = Params.New()
                .CustomString(JsonUtility.ToJson(versionData));
            
            Elephant.Event("elephant_sdk_versions_info", -1, parameters);
        }

        private void OnFbInitComplete()
        {
            if (FB.IsInitialized) {
                FB.ActivateApp();
                FB.Mobile.SetAdvertiserIDCollectionEnabled(false);
                FB.Mobile.SetAdvertiserTrackingEnabled(false);
            } else {
                ElephantLog.Log("ELEPHANT INIT","Failed to Initialize the Facebook SDK");
            }
        }
        
        public OpenResponse GetOpenResponse()
        {
            return openResponse;
        }

        private void RequestIDFAAndOpen()
        {
            idfv = SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
            idfa = "UNITY_EDITOR_IDFA";
            StartCoroutine(OpenRequest());
#elif UNITY_IOS
            idfa = ElephantIOS.IDFA();
            consentStatus = ElephantIOS.getConsentStatus();
            StartCoroutine(OpenRequest());
#elif UNITY_ANDROID
            idfa = ElephantAndroid.FetchAdId();
            StartCoroutine(OpenRequest());
#else
            idfa = "UNITY_UNKOWN_IDFA";
            StartCoroutine(OpenRequest());
#endif
        }

        private IEnumerator OpenRequest()
        {
            // initialized event
            if (onInitialized != null)
                onInitialized();


            currentSession = SessionData.CreateSessionData();
            realSessionId = currentSession.GetSessionID();
            SendVersionsEvent();

            var openData = OpenData.CreateOpenData();
            openData.session_id = currentSession.GetSessionID();
            openData.idfv = idfv;
            openData.idfa = idfa;
            openData.user_id = userId;
            cachedOpenResponse = Utils.ReadFromFile(CACHED_OPEN_RESPONSE);
            if (!string.IsNullOrEmpty(cachedOpenResponse))
            {
                var tempOpenResponse = JsonUtility.FromJson<OpenResponse>(cachedOpenResponse);
                if (tempOpenResponse != null)
                {
                    // Previous open response has successfully saved. Send Hash..
                    openData.hash = tempOpenResponse.hash;
                }
            }

            var json = JsonUtility.ToJson(openData);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<OpenResponse>();
            var postWithResponse = networkManager.PostWithResponse(OPEN_EP, bodyJson, response =>
            {
                if (response.responseCode == 200)
                {
                    if (response.data != null)
                    {
                        openRequestSucceded = true;
                        openResponse = response.data;
                    }
                }
                else if (response.responseCode == 204)
                {
                    var data = JsonUtility.FromJson<OpenResponse>(cachedOpenResponse);
                    if (data != null)
                    {
                        Elephant.Event("hashed_open_response", -1);
                        openRequestSucceded = true;
                        openResponse = data;
                    }
                }
                
                openRequestWaiting = false;
            }, s =>
            {
                openRequestWaiting = false;
            });
            
            yield return postWithResponse;
        }

        public void CheckApiHealth()
        {
            if (ElephantCore.Instance == null) return;
            
            var bodyJson = JsonUtility.ToJson(new ElephantData("", ElephantCore.Instance.GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<HealthCheckResponse>();
            var postWithResponse = networkManager.PostWithResponse(HEALTH_CHECK_EP, bodyJson, response =>
            {
                if (response.responseCode != 200)
                {
                    circuitBreakerEnabled = true;
                }
                else
                {
                    circuitBreakerEnabled = false;
                    if (response.data == null) return;
                    
                    health_check_retry_period = response.data.retry_period;
                    fail_retry_count = response.data.retry_count;

                }
            }, s =>
            {
                
            });
            StartCoroutine(postWithResponse);
        }
        
        public void PinRequest()
        {
#if UNITY_EDITOR
            // No-op
            ElephantLog.Log("COMPLIANCE TEST","showPopUpView Loading");
#elif UNITY_IOS
            ElephantIOS.showPopUpView("LOADING", "", "", "", "", "", "", "", "");
#elif UNITY_ANDROID
            ElephantAndroid.ShowConsentDialogOnUiThread("LOADING", "", "", "", "", "", "", "", "");
#endif
            
            var data = new ComplianceRequestData();
            var json = JsonUtility.ToJson(data);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, ElephantCore.Instance.GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<Pin>();
            var postWithResponse = networkManager.PostWithResponse(PIN_EP, bodyJson, response =>
            {
                var pinData = response.data;
                
                if (pinData != null)
                {
#if UNITY_EDITOR
                    // No-op
                    ElephantLog.Log("COMPLIANCE TEST","showPopUpView Content");
#elif UNITY_IOS
                    ElephantIOS.showPopUpView("CONTENT", pinData.content, "Go Back", pinData.privacy_policy_text, pinData.privacy_policy_url,
                            pinData.terms_of_service_text, pinData.terms_of_service_url, pinData.data_request_text,
                            pinData.data_request_url);
#elif UNITY_ANDROID
                    ElephantAndroid.ShowConsentDialogOnUiThread("CONTENT", pinData.content, "Go Back",
                            pinData.privacy_policy_text, pinData.privacy_policy_url, pinData.terms_of_service_text,
                            pinData.terms_of_service_url, pinData.data_request_text, pinData.data_request_url);
#endif                    
                }
                else
                {
#if UNITY_EDITOR
                    // No-op
                    ElephantLog.Log("COMPLIANCE TEST","showPopUpView Error");
#elif UNITY_IOS
            ElephantIOS.showPopUpView("ERROR", "", "", "", "", "", "", "", "");
#elif UNITY_ANDROID
                    ElephantAndroid.ShowConsentDialogOnUiThread("ERROR", "", "", "", "", "", "", "", "");
#endif  
                }
            }, s =>
            {
#if UNITY_EDITOR
                    // No-op
                    ElephantLog.Log("COMPLIANCE TEST","showPopUpView Error");
#elif UNITY_IOS
                    ElephantIOS.showPopUpView("ERROR", "", "", "", "", "", "", "", "");
#elif UNITY_ANDROID
                    ElephantAndroid.ShowConsentDialogOnUiThread("ERROR", "", "", "", "", "", "", "", "");
#endif 
            });
            StartCoroutine(postWithResponse);
        }

        public void VerifyPurchase(IapVerifyRequest request, Action<bool> callback)
        {
            var json = JsonUtility.ToJson(request);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, Instance.GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<IapVerification>();
            var postWithResponse = networkManager.PostWithResponse(IAP_VERIFY_EP, bodyJson, response =>
            {
                var responseData = response.data;
                if (responseData != null)
                {
                    callback(responseData.verified);
                }
            }, s =>
            {
                callback(false);
            });
            StartCoroutine(postWithResponse);
        }
        
        public void IsIapBanned(Action<bool, string> callback)
        {
            StartCoroutine(IsIapBannedRequest(callback));
        }

        private IEnumerator IsIapBannedRequest(Action<bool, string> callback)
        {
            var iapStatusRequest = IapStatusRequest.Create();

            var json = JsonUtility.ToJson(iapStatusRequest);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, GetCurrentSession().GetSessionID()));

            using (var request = new UnityWebRequest(IAP_STATUS_EP, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Content-Encoding", "gzip");
                request.SetRequestHeader("Authorization", Utils.SignString(bodyJson, GameSecret));
                request.SetRequestHeader("GameID", GameID);

                yield return request.SendWebRequest();

                if (request.isNetworkError)
                {
                    ElephantLog.LogError("IAP CHECK", "Request failed with network error");
                    callback(true, "Something went wrong. Please try again.");
                    Elephant.ShowAlertDialog("Error", "Something went wrong. Please try again.");
                }
                else
                {
                    try
                    {
                        var iapStatusResponse = JsonUtility.FromJson<IapStatusResponse>(request.downloadHandler.text);
                        if (iapStatusResponse != null)
                        {
                            isIapBanned = iapStatusResponse.is_banned;
                            callback(isIapBanned, iapStatusResponse.message);
                            if (iapStatusResponse.is_banned)
                            {
                                // Sending ToS link via title param.
                                Elephant.ShowAlertDialog(iapStatusResponse.link, iapStatusResponse.message);
                            }

                        }
                        else
                        {
                            callback(true, "Something went wrong. Please try again.");
                            Elephant.ShowAlertDialog("Error", "Something went wrong. Please try again.");
                        }
                    }
                    catch (Exception e)
                    {
                        callback(true, "Something went wrong. Please try again.");
                        Elephant.ShowAlertDialog("Error", "Something went wrong. Please try again.");
                        ElephantLog.LogError("IAP CHECK", "Request failed with error: " + e.Message);
                    }
                }
            }
        }

        public SessionData GetCurrentSession()
        {
            return currentSession;
        }

        public void AddToQueue(ElephantRequest data)
        {
            this._queue.Enqueue(data);
        }
        
        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                var savedTimeSpend = Utils.ReadFromFile(TimeSpend);
                timeSpend = !string.IsNullOrEmpty(savedTimeSpend) ? long.Parse(savedTimeSpend) : 0;
                
                currentSession = SessionData.CreateSessionData();
                

                // Reset real session id and event order if necessary time passes
                // Sometimes unity won't reset game after a long focus-free session
                if (Time.unscaledDeltaTime - Instance.focusLostTime > InternalConfig.GetInstance().focus_interval)
                {
                    Instance.realSessionId = currentSession.GetSessionID();
                    Instance.eventOrder = 0;
                }

                ElephantLog.Log("APP STATE","Focus Gained");
                // rebuild queues from disk..
                RebuildQueue();

                // start queue processing
                processQueues = true;
            }
            else
            {
                var currentSessionTS = Utils.Timestamp() - currentSession.GetSessionID();
                var totalTS = timeSpend + currentSessionTS;
                Utils.SaveToFile(TimeSpend, totalTS.ToString());
                
                // Time saved for next focus
                Instance.focusLostTime = Time.unscaledDeltaTime;
                
                ElephantLog.Log("APP STATE","Focus Lost");
                // pause late update
                processQueues = false;

                // send session log
                var sessionEndCurrentSession = ElephantCore.Instance.GetCurrentSession();
                sessionEndCurrentSession.RefreshBaseData();
                sessionEndCurrentSession.end_time = Utils.Timestamp();

                var sessionReq = new ElephantRequest(SESSION_EP, sessionEndCurrentSession);
                AddToQueue(sessionReq);
                
                var monitoringReq = new ElephantRequest(MONITORING_EP, MonitoringData.CreateMonitoringData());
                AddToQueue(monitoringReq);

                // process queues
                ProcessQueues(true);

                // drain queues and persist them to send after gaining focus
                SaveQueues();
            }
        }

        private void RebuildQueue()
        {
            string json = Utils.ReadFromFile(QUEUE_DATA_FILE);
            if (json != null)
            {
                ElephantLog.Log("APP STATE","QUEUE <- " + json);
                var d = JsonUtility.FromJson<QueueData>(json);
                if (d?.queue != null)
                {
                    _failedQueue = d.queue;
                    foreach (var r in _failedQueue)
                    {
                        r.tryCount = 0;
                    }
                }
            }
        }

        private void SaveQueues()
        {
            while (_queue.Count > 0)
            {
                ElephantRequest data = _queue.Dequeue();
                _failedQueue.Add(data);
            }

            var queueJson = JsonUtility.ToJson(new QueueData(_failedQueue));
            ElephantLog.Log("APP STATE","QUEUE -> " + queueJson);

            Utils.SaveToFile(QUEUE_DATA_FILE, queueJson);

            _failedQueue.Clear();
        }

        private void LateUpdate()
        {
            ProcessQueues(false);
        }


        private void ProcessQueues(bool forceToSend)
        {
            if (forceToSend || (processQueues && sdkIsReady))
            {

                if (InternalConfig.GetInstance().request_logic_enabled)
                {
                    if (processFailedBatch)
                    {
                        StartCoroutine(BatchPost());
                    }
                }
                else
                {
                    int failedCount = _failedQueue.Count;
                    for (int i = failedCount - 1; i >= 0; --i)
                    {
                        ElephantRequest data = _failedQueue[i];
                        int tc = data.tryCount % 6;
                        int backoff = (int) (Math.Pow(2, tc) * 1000);

                        if (Utils.Timestamp() - data.lastTryTS > backoff)
                        {
                            _failedQueue.RemoveAt(i);
                            StartCoroutine(Post(data));
                        }
                    }
                }
                

                while (_queue.Count > 0)
                {
                    ElephantRequest data = _queue.Dequeue();
                    StartCoroutine(Post(data));
                }
            }
        }

        IEnumerator BatchPost()
        {
            if (_failedQueue.Count == 0) yield break;
            processFailedBatch = false;

            while (_failedQueue.Count > 0)
            {
                var counter = 0;
                var listCounter = _failedQueue.Count - 1;
                ElephantLog.Log("BatchPost", "start new batch ");
                
                while (counter < 10 && listCounter >= 0)
                {
                    var request = _failedQueue[listCounter];
                    int tc = request.tryCount % 6;
                    int backoff = (int) (Math.Pow(2, tc) * 1000);
                    

                    if (Utils.Timestamp() - request.lastTryTS > backoff)
                    {
                        _failedQueue.RemoveAt(listCounter);
                        if (!circuitBreakerEnabled)
                        {
                            ElephantLog.Log("BatchPost", "request: " + request.url);
                            ElephantLog.Log("BatchPost", "batch count: " + _failedQueue.Count);
                            StartCoroutine(Post(request));
                        }
                    }
                    
                    counter++;
                    listCounter--;

                }

                ElephantLog.Log("BatchPost", "wait 10: ");
                yield return new WaitForSeconds(3);
            }

            processFailedBatch = true;
        }

        IEnumerator Post(ElephantRequest elephantRequest)
        {
            ElephantLog.Log("POST WITH F&F",elephantRequest.tryCount + " - " + (Utils.Timestamp() - elephantRequest.lastTryTS) + " -> " +
                                        elephantRequest.url + " : " + elephantRequest.data);

            if (InternalConfig.GetInstance().reachability_check_enabled)
            {
                Elephant.ShowNetworkOfflineDialog();
                
                while (!Utils.IsConnected())
                {
                    yield return null;
                }
            }

            elephantRequest.tryCount++;
            elephantRequest.lastTryTS = Utils.Timestamp();

            var elephantData = new ElephantData(elephantRequest.data, GetCurrentSession().GetSessionID(), elephantRequest.isOffline, elephantRequest.statusCode > 0);

            string bodyJsonString = JsonUtility.ToJson(elephantData);

            string authToken = Utils.SignString(bodyJsonString, GameSecret);


#if UNITY_EDITOR

            using (var request = new UnityWebRequest(elephantRequest.url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Content-Encoding", "gzip");
                request.SetRequestHeader("Authorization", authToken);
                request.SetRequestHeader("GameID", ElephantCore.Instance.GameID);

                yield return request.SendWebRequest();

                ElephantLog.Log("POST WITH F&F", "Status Code: " + request.responseCode);

                if (request.responseCode != 200)
                {
                    // failed will be retried
                    if (_failedQueue.Count < MAX_FAILED_COUNT)
                    {
                        _failedQueue.Add(elephantRequest);
                    }
                    else
                    {
                        ElephantLog.LogError("POST WITH F&F", "Failed Queue size -> " + _failedQueue.Count);
                    }
                }
            }

#else
#if UNITY_IOS
            ElephantIOS.ElephantPost(elephantRequest.url, bodyJsonString, GameID, authToken, elephantRequest.tryCount);
#elif UNITY_ANDROID
            ElephantAndroid.ElephantPost(elephantRequest.url, bodyJsonString, GameID, authToken, elephantRequest.tryCount);
#endif
            yield return null;
#endif
        }
        
        // Triggered from native plugins
        public void ReferralData(string referralDataJson)
        {
            try
            {
                var param = Params.New().CustomString(referralDataJson);
                Elephant.Event("referralData", -1, param);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void FailedRequest(string reqJson)
        {
            try
            {
                var req = JsonUtility.FromJson<ElephantRequest>(reqJson);
                req.lastTryTS = Utils.Timestamp();

                // trick..
                var body = JsonUtility.FromJson<ElephantData>(req.data);
                req.data = body.data;
                
                if (_failedQueue.Count < MAX_FAILED_COUNT)
                {
                    _failedQueue.Add(req);
                }
                else
                {
                    ElephantLog.Log("POST WITH F&F","Failed Queue size -> " + _failedQueue.Count);
                }
            }
            catch (Exception e)
            {
                ElephantLog.Log("POST WITH F&F",e.Message);
            }
        }

        void setConsentStatus(string message)
        {
#if UNITY_IOS && !UNITY_EDITOR
            idfa = ElephantIOS.IDFA();
#endif
            triggerConsentResult(message);
            var parameters = Params.New();
            parameters.Set("status", message);
            Elephant.Event("idfa_consent_change", -1, parameters);
            consentStatus = message;
        }
        
        void sendUiConsentStatus(string message)
        {
            if (message.Equals("denied"))
            {
                triggerConsentResult(message);
            }
            var parameters = Params.New();
            parameters.Set("status", message);
            Elephant.Event("idfa_ui_consent_change", -1, parameters);
            consentStatus = message;
        }

        void triggerConsentResult(string message)
        {
            var parameters = Params.New();
            parameters.Set("status", message);
            Elephant.Event("set_idfa_consent_result", -1, parameters);
            IdfaConsentResult.GetInstance().SetIdfaResultValue(message);
            IdfaConsentResult.GetInstance().SetStatus(IdfaConsentResult.Status.Resolved);

#if UNITY_IOS
            AdjustConfig config = new AdjustConfig(ElephantThirdPartyIds.AdjustAppKey, AdjustEnvironment.Production);
            config.setAttributionChangedDelegate(OnAttrChange);
            Adjust.start(config);

            _elephantComplianceManager.ShowCcpa();
            _elephantComplianceManager.ShowGdprAdConsent();
#endif 
        }
        
        public void UserConsentAction(string userAction)
        {
            switch (userAction)
            {
                case "TOS_ACCEPT":
                    _elephantComplianceManager.SendTosAccept();
                    break;
                case "GDPR_AD_CONSENT_AGREE":
                    _elephantComplianceManager.SendGdprAdConsentStatus(true);
                    break;
                case "GDPR_AD_CONSENT_DECLINE":
                    _elephantComplianceManager.SendGdprAdConsentStatus(false);
                    break;
                case "PERSONALIZED_ADS_AGREE":
                    _elephantComplianceManager.SendCcpaStatus(true);
                    break;
                case "PERSONALIZED_ADS_DECLINE":
                    _elephantComplianceManager.SendCcpaStatus(false);
                    break;
                case "CALL_DATA_REQUEST":
                    PinRequest();
                    break;
                case "DELETE_REQUEST_CANCEL":
                    CreateNewUser(response =>
                    {
                        if (response.responseCode != 200) return;

                        var openResponseForNewUser = response.data;
                        this.userId = openResponseForNewUser.user_id;
                        _elephantComplianceManager.UpdateOpenResponse(openResponseForNewUser);
                        _elephantComplianceManager.ShowTosAndPp(onOpen);

                    }, s =>
                    {
                        ElephantLog.Log("COMPLIANCE", "Error on new user creation: " + s);
                    });
                    break;
                case "RETRY_CONNECTION":
                    if (Utils.IsConnected())
                    {
                        Utils.ResumeGame();
                    }
                    else
                    {
                        Elephant.ShowNetworkOfflineDialog();
                    }
                    break;
            }
        }

        public void GetSettingsContent(Action<GenericResponse<SettingsResponse>> onResponse, Action<string> onError)
        {
            var data = new BaseData();
            data.FillBaseData(Instance.GetCurrentSession().GetSessionID());
            var json = JsonUtility.ToJson(data);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, Instance.GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<SettingsResponse>();
            var postWithResponse = networkManager.PostWithResponse(SETTINGS_EP, bodyJson, onResponse, onError);

            StartCoroutine(postWithResponse);
        }
        
        public void CreateNewUser(Action<GenericResponse<OpenResponse>> onResponse, Action<string> onError)
        {
            var data = new NewUserRequest();
            data.FillBaseData(Instance.GetCurrentSession().GetSessionID());
#if UNITY_EDITOR
            data.locale = CultureInfo.CurrentCulture.Name;
#elif UNITY_IOS
            data.locale = ElephantIOS.getLocale();
#elif UNITY_ANDROID
            data.locale = ElephantAndroid.GetLocale();
#else
            data.locale = CultureInfo.CurrentCulture.Name;
#endif
            var json = JsonUtility.ToJson(data);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, Instance.GetCurrentSession().GetSessionID()));
            var networkManager = new GenericNetworkManager<OpenResponse>();
            var postWithResponse = networkManager.PostWithResponse(USER_EP, bodyJson, onResponse, onError);
            
            this.userId = "";
            Utils.SaveToFile(USER_DB_ID, "");
            StartCoroutine(postWithResponse);
        }
        
        public static void StartAdManager()
        {
            Assembly assemblyForAds = Assembly.GetExecutingAssembly();
            foreach (var type in assemblyForAds.GetTypes())
            {
                if (type.FullName == null) return;
                
                if (type.FullName.Equals("RollicGames.Advertisements.RLAdManager"))
                {
                    MethodInfo info = type.GetMethod("StartAdManager");
                    object classInstance = Activator.CreateInstance(type, null);

                    if (info is null) return;
                    
                    info.Invoke(classInstance, null);
                    
                    var evnt = OnStartGame;
                    evnt?.Invoke();
                }
            }
        }
    }
}