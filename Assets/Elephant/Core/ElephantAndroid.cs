using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantAndroid
{
    #if UNITY_ANDROID
    
    private static AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    private static AndroidJavaObject elephantController;
    
    public static void Init()
    {
        try
        {
            AndroidJavaClass elephantCoreClass = new AndroidJavaClass("com.rollic.elephantsdk.ElephantController");
            ElephantAndroid.elephantController = elephantCoreClass.CallStatic<AndroidJavaObject>("create", currentActivity);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
    }
    
    public static void showAlertDialog(string title, string message)
    {
        if (elephantController != null)
        {
            elephantController.Call("showAlertDialog", title, message);    
        }
    }

    public static void showForceUpdate(string title, string message)
    {
        if (elephantController != null)
        {
            elephantController.Call("showForceUpdate", title, message);    
        }
    }

    public static void ElephantPost(string url, string body, string gameID, string authToken, int tryCount)
    {
        if (elephantController != null)
        {
            elephantController.Call("ElephantPost", url, body, gameID, authToken, tryCount);    
        }
    }

    public static string getBuildNumber()
    {
        var buildNumber = "";
        if (elephantController != null)
        {
            buildNumber = elephantController.Call<string>("getBuildNumber");    
        }

        return buildNumber;
    }

    public static string GetLocale()
    {
        var locale = "";
        if (elephantController != null)
        {
            locale = elephantController.Call<string>("getLocale");    
        }

        return locale;
    }
    
     public static string FetchAdId()
    {
        var adId = "";
        if (elephantController != null)
        {
            adId = elephantController.Call<string>("FetchAdId");
        }

        return adId;
    }
    
    public static void ShowConsentDialogOnUiThread(string dialogSubviewType, string text, string buttonTitle, string privacyPolicyText, 
        string privacyPolicyUrl, string termsOfServiceText, string termsOfServiceUrl, string dataRequestText = "", string dataRequestUrl = "")
    {
        AndroidJavaRunnable runnable = () =>
        {
            if (elephantController != null)
            {
                elephantController.Call("showConsent", dialogSubviewType, text, buttonTitle, privacyPolicyText, privacyPolicyUrl, termsOfServiceText, termsOfServiceUrl, dataRequestText, dataRequestUrl);
            }
        };

        currentActivity.Call("runOnUiThread", runnable);
    }

    public static void ShowCcpaDialog(string action, string title, string content, string privacyPolicyText, 
        string privacyPolicyUrl, string declineActionButtonText, string agreeActionButtonText, string backToGameActionButtonText)
    {
        if (elephantController != null)
        {
            elephantController.Call("showCcpaDialog", action, title, content, privacyPolicyText, 
                privacyPolicyUrl, declineActionButtonText, agreeActionButtonText, backToGameActionButtonText);
        }
    }
    
    public static void ShowReturningUserDialog(string action, string title, string content, string privacyPolicyText, 
        string privacyPolicyUrl, string backToGameButtonText)
    {
        if (elephantController != null)
        {
            elephantController.Call("showReturningUserDialog", action, title, content, privacyPolicyText, 
                privacyPolicyUrl, backToGameButtonText);
        }
    }
    
    public static void ShowSettingsViewOnUiThread(string dialogSubviewType, string actions)
    {
        AndroidJavaRunnable runnable = new AndroidJavaRunnable(() =>
        {
            if (elephantController != null)
            {
                elephantController.Call("showSettingsView", dialogSubviewType, actions);
            }
        });
        
        currentActivity.Call("runOnUiThread", runnable);
    }

    public static void showBlockedDialog(string title, string content, string warningContent, string buttonTitle)
    {
        if (elephantController != null)
        {
            elephantController.Call("showBlockedDialog", title, content, warningContent, buttonTitle);
        }
    }

    public static void ShowNetworkOfflineDialog(string content, string buttonTitle)
    {
        AndroidJavaRunnable runnable = new AndroidJavaRunnable(() =>
        {
            if (elephantController != null)
            {
                elephantController.Call("showNetworkOfflineDialog", content, buttonTitle);
            }
        });

        currentActivity.Call("runOnUiThread", runnable);
    }
    
    public static int GameMemoryUsage()
    {
        var memoryUsage = 1;
        if (elephantController != null)
        {
            memoryUsage = elephantController.Call<int>("gameMemoryUsage");
        }
        return memoryUsage;
    }
    
    public static int GameMemoryUsagePercentage()
    {
        var memoryUsagePercentage = 1;
        if (elephantController != null)
        {
            memoryUsagePercentage = elephantController.Call<int>("gameMemoryUsagePercentage");
        }
        return memoryUsagePercentage;
    }

    public static float GetBatteryLevel()
    {
        float batteryLevel = 0;
        if (elephantController != null)
        {
            batteryLevel = elephantController.Call<float>("getBatteryLevel");
        }
        return batteryLevel;
    }
    
    public static long GetFirstInstallTime()
    {
        long firstInstallTime = 0;
        if (elephantController != null)
        {
            firstInstallTime = elephantController.Call<long>("getFirstInstallTime");
        }
        return firstInstallTime;
    }
    
    #endif
}
