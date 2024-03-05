using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElephantSDK
{
    public class ElephantUI : MonoBehaviour
    {
        private GameObject loaderUI;

        public static ElephantUI Instance;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Application.targetFrameRate = 60;
            Init();
        }


        public void Init()
        {
            loaderUI = GameObject.Find("loader");

            ShowLoaderUI();

            ElephantCore.onInitialized += OnInitialized;
            ElephantCore.onOpen += OnOpen;
            ElephantCore.onRemoteConfigLoaded += OnRemoteConfigLoaded;

            bool isOldUser = false;
            Elephant.Init(isOldUser, true);
        }

        void OnInitialized()
        {
            ElephantLog.Log("INIT","Elephant Initialized");
        }
        
        void OnOpen(bool gdprRequired, ComplianceTosResponse tos)
        {
            Debug.Log("Elephant Open Result, we can start the game or show gdpr -> " + gdprRequired);
            if (gdprRequired)
            {
#if UNITY_EDITOR
                // no-op
                PlayGame();
                ElephantLog.Log("COMPLIANCE", "ShowToSAndPPDialog");
#elif UNITY_ANDROID
                 ElephantAndroid.ShowConsentDialogOnUiThread("CONTENT", tos.content, tos.consent_text_action_button, tos.privacy_policy_text,
                    tos.privacy_policy_url, tos.terms_of_service_text, tos.terms_of_service_url);
#elif UNITY_IOS
                ElephantIOS.showPopUpView("CONTENT", tos.content, tos.consent_text_action_button, tos.privacy_policy_text,
                    tos.privacy_policy_url, tos.terms_of_service_text, tos.terms_of_service_url);
#else
                // no-op
#endif
            }
            else
            {
                PlayGame();
            }
        }

        void OnRemoteConfigLoaded()
        {
            Debug.Log(
                "Elephant Remote Config Loaded, we can retrieve configuration params via RemoteConfig.GetInstance().Get() or other variant methods..");
        }


        private void ShowLoaderUI()
        {
            loaderUI.SetActive(true);
        }

        public void PlayGame()
        {
            
#if UNITY_EDITOR
            Debug.LogWarning("You cannot initialize AdManager in Editor mode");
#else
            ElephantCore.StartAdManager();
#endif

//#if !ELEPHANT_DEBUG
            SceneManager.LoadScene(1);
//#endif
        }
    }
}