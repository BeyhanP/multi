using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    public class Elephant
    {
        private static string LEVEL_STARTED = "level_started";
        private static string LEVEL_FAILED = "level_failed";
        private static string LEVEL_COMPLETED = "level_completed";

        public static event Action OnLevelCompleted;
        public static void Init(bool isOldUser = false, bool gdprSupported = false)
        {
            ElephantCore.Instance.Init();
        }
        
        public static IEnumerator ResetSoundWithDelay()
        {
            if (!ElephantCore.Instance.isSoundFixEnabled)
                yield break;

            yield return new WaitForSeconds(3);
            AudioSettings.Reset(AudioSettings.GetConfiguration());
        }

        public static void ShowComplianceDialog()
        {
            ElephantCore.Instance.PinRequest();
        }

        public static void ShowSupportView(string subject, string body)
        {
            subject = Utils.ReplaceEscapeCharsForUrl(subject);
            body = body + "\n\n" + "Elephant ID: " + ElephantCore.Instance.userId + "\nIDFV: " +
                   ElephantCore.Instance.idfv;
            body = Utils.ReplaceEscapeCharsForUrl(body);
            Application.OpenURL("mailto:" + "support@rollicgames.com" + "?subject=" + subject + "&body=" + body);
        }

        public static void IsIapBanned(Action<bool, string> callback)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            if (callback == null)
            {
                Debug.LogError("IsIapBanned callback cannot be null!");
                return;
            }

            ElephantCore.Instance.IsIapBanned(callback);
        }

        public static void VerifyPurchase(IapVerifyRequest request, Action<bool> callback)
        {
            if (ElephantCore.Instance == null)
            {
                ElephantLog.LogError("<ELEPHANT>", "Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            if (callback == null)
            {
                throw new NullReferenceException("ValidatePurchase callback cannot be null!");
            }

            ElephantCore.Instance.VerifyPurchase(request, callback);
        }

        public static void ShowAlertDialog(string title, string message)
        {
#if UNITY_IOS
            ElephantIOS.showAlertDialog(title, message);
#elif UNITY_ANDROID
            ElephantAndroid.showAlertDialog(title, message);
#else
            Debug.LogError(message);
#endif
        }

        public static void LevelStarted(int level, Params parameters = null)
        {
            MonitoringUtils.GetInstance().SetCurrentLevel(level, level.ToString());

            CustomEvent(LEVEL_STARTED, level, originalLevelId: level.ToString(), param: parameters);
        }

        public static void LevelCompleted(int level, Params parameters = null)
        {
            var currentLevel = MonitoringUtils.GetInstance().GetCurrentLevel();
            var currentTime = Utils.Timestamp();
            var levelTime = currentTime - currentLevel.level_time;

            CustomEvent(LEVEL_COMPLETED, level, level.ToString(), levelTime, parameters);

            if (OnLevelCompleted == null) return;
            var evnt = OnLevelCompleted;
            evnt?.Invoke();
        }

        public static void LevelFailed(int level, Params parameters = null)
        {
            var currentLevel = MonitoringUtils.GetInstance().GetCurrentLevel();
            var currentTime = Utils.Timestamp();
            var levelTime = currentTime - currentLevel.level_time;

            CustomEvent(LEVEL_FAILED, level, originalLevelId: level.ToString(), levelTime: levelTime, param: parameters);
        }

        public static void Event(string type, int level, Params parameters = null)
        {
            CustomEvent(type, level, param: parameters);
        }

        public static void AdEventV2(string type, string json)
        {
            if (!AdConfig.GetInstance().ad_callback_logs) return;

            var param = Params.New();
            param.CustomString(json);

            CustomEvent("AdEvent_" + type, -1, param: param);
        }

        public static void RewardedEvent(string eventType, ElephantLevel level, string type, string source, string item, string adUuid, int result = -1, string mediationInfo = "")
        {
            var parameters = Params.New()
                .Set("ad_placement_category", type)
                .Set("ad_placement_source", source)
                .Set("ad_placement_item", item)
                .Set("mediation_info", mediationInfo + "|" + adUuid)
                .Set("result", result);

            CustomEvent(eventType, level.level, originalLevelId: level.original_level, param: parameters);
        }

        public static void InterstitialEvent(string eventType, ElephantLevel level, string source, string adUuid, int result = -1, string mediationInfo = "")
        {
            var parameters = Params.New()
                .Set("ad_placement_source", source)
                .Set("mediation_info", mediationInfo + "|" + adUuid)
                .Set("result", result);

            CustomEvent(eventType, level.level, originalLevelId: level.original_level, param: parameters);
        }

        public static void Transaction(string type, int level, long amount, long finalAmount, string source)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            var t = TransactionData.CreateTransactionData();
            t.type = type;
            t.level = level;
            t.amount = amount;
            t.final_amount = finalAmount;
            t.source = source;

            var req = new ElephantRequest(ElephantCore.TRANSACTION_EP, t);
            ElephantCore.Instance.AddToQueue(req);
        }

        [Obsolete("This Method is Deprecated, use MediationAdRevenueEvent")]
        public static void AdRevenueEvent(string mopubRevenueData)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            var adRevenueRequest = AdRevenueRequest.CreateAdRevenueRequest(mopubRevenueData);

            var req = new ElephantRequest(ElephantCore.AD_REVENUE_EP, adRevenueRequest);
            ElephantCore.Instance.AddToQueue(req);
        }

        public static void MediationAdRevenueEvent(string mediationRevenueData)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            var adRevenueRequest = AdRevenueRequest.CreateMediationRevenueRequest(mediationRevenueData);

            var req = new ElephantRequest(ElephantCore.AD_REVENUE_EP, adRevenueRequest);
            ElephantCore.Instance.AddToQueue(req);
        }

        // For IS integrated apps.
        public static void IronsourceAdRevenueEvent(string ironsourceRevenueData)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            var adRevenueRequest = AdRevenueRequest.CreateIronSourceAdRevenueRequest(ironsourceRevenueData);

            var req = new ElephantRequest(ElephantCore.AD_REVENUE_EP, adRevenueRequest);
            ElephantCore.Instance.AddToQueue(req);
        }

        private static void CustomEvent(string type, int level, string originalLevelId = "", long levelTime = 0, Params param = null)
        {
            if (ElephantCore.Instance == null)
            {
                Debug.LogWarning("Elephant SDK isn't working correctly, make sure you put Elephant prefab into your first scene..");
                return;
            }

            var ev = EventData.CreateEventData();
            ev.type = type;
            ev.level = level;
            ev.level_id = originalLevelId;
            ev.level_time = levelTime;

            if (param != null)
            {
                MapParams(param, ev);
            }

            var req = new ElephantRequest(ElephantCore.EVENT_EP, ev);
            ElephantCore.Instance.AddToQueue(req);
        }

        public static void ShowSettingsView()
        {
#if UNITY_EDITOR
// No-op
#elif UNITY_IOS
                    ElephantIOS.showSettingsView("LOADING", "");
#elif UNITY_ANDROID
                    ElephantAndroid.ShowSettingsViewOnUiThread("LOADING", "");
#endif

            ElephantCore.Instance.GetSettingsContent(response =>
            {
                if (response.responseCode == 200)
                {
                    string responseString = JsonUtility.ToJson(response.data);

#if UNITY_EDITOR
// No-op
#elif UNITY_IOS
                    ElephantIOS.showSettingsView("CONTENT", responseString);
#elif UNITY_ANDROID
                    ElephantAndroid.ShowSettingsViewOnUiThread("CONTENT", responseString);
#endif
                }
                else
                {
#if UNITY_EDITOR
// No-op
#elif UNITY_IOS
                    ElephantIOS.showSettingsView("ERROR", "");
#elif UNITY_ANDROID
                    ElephantAndroid.ShowSettingsViewOnUiThread("ERROR", "");
#endif

                    Debug.Log("Settings Error: " + response.responseCode + " " + response.errorMessage);
                }
            }, s =>
            {
#if UNITY_EDITOR
// No-op
#elif UNITY_IOS
                    ElephantIOS.showSettingsView("ERROR", "");
#elif UNITY_ANDROID
                    ElephantAndroid.ShowSettingsViewOnUiThread("ERROR", "");
#endif

                Debug.Log("Settings Error: " + s);
            });
        }


        public static void ShowNetworkOfflineDialog()
        {
            if (!Utils.IsConnected())
            {
                if (ElephantCore.Instance != null)
                {
                    Utils.SaveToFile(ElephantCore.OFFLINE_FLAG, ElephantCore.Instance.GetCurrentSession().GetSessionID().ToString());
                }
#if UNITY_EDITOR
                ElephantLog.Log("Connection", "No internet connection.\nPlease check your internet settings.");
#elif UNITY_IOS
                ElephantIOS.showNetworkOfflinePopUpView("No internet connection.\nPlease check your internet settings.", "Try Again");
#elif UNITY_ANDROID
                ElephantAndroid.ShowNetworkOfflineDialog("No internet connection.\nPlease check your internet settings.", "Try Again");
#endif

                Utils.PauseGame();
            }
        }

        private static void MapParams(Params param, EventData ev)
        {
            ev.custom_data = param.customData;

            int c = 0;
            foreach (var k in param.stringVals.Keys)
            {
                var v = param.stringVals[k];
                if (c == 0)
                {
                    ev.key_string1 = k;
                    ev.value_string1 = v;
                }
                else if (c == 1)
                {
                    ev.key_string2 = k;
                    ev.value_string2 = v;
                }
                else if (c == 2)
                {
                    ev.key_string3 = k;
                    ev.value_string3 = v;
                }
                else if (c == 3)
                {
                    ev.key_string4 = k;
                    ev.value_string4 = v;
                }

                c++;
            }


            c = 0;
            foreach (var k in param.intVals.Keys)
            {
                var v = param.intVals[k];
                if (c == 0)
                {
                    ev.key_int1 = k;
                    ev.value_int1 = v;
                }
                else if (c == 1)
                {
                    ev.key_int2 = k;
                    ev.value_int2 = v;
                }
                else if (c == 2)
                {
                    ev.key_int3 = k;
                    ev.value_int3 = v;
                }
                else if (c == 3)
                {
                    ev.key_int4 = k;
                    ev.value_int4 = v;
                }

                c++;
            }

            c = 0;
            foreach (var k in param.doubleVals.Keys)
            {
                var v = param.doubleVals[k];
                if (c == 0)
                {
                    ev.key_double1 = k;
                    ev.value_double1 = v;
                }
                else if (c == 1)
                {
                    ev.key_double2 = k;
                    ev.value_double2 = v;
                }
                else if (c == 2)
                {
                    ev.key_double3 = k;
                    ev.value_double3 = v;
                }
                else if (c == 3)
                {
                    ev.key_double4 = k;
                    ev.value_double4 = v;
                }

                c++;
            }
        }

        public class IapSource
        {
            public const string SOURCE_SHOP = "shop";
            public const string SOURCE_OFFERWALL = "offerwall";
            public const string SOURCE_IN_GAME = "in_game";
            public const string SOURCE_LEVEL_END = "level_end";
        }
    }
}