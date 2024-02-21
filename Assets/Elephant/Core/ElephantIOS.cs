using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ElephantIOS 
{
   
    #if UNITY_IOS
    [DllImport ("__Internal")]
    public static extern void TestFunction(string a);
    
    [DllImport ("__Internal")]
    public static extern string IDFA();
    
    [DllImport ("__Internal")]
    public static extern void ElephantPost(string url, string body, string gameID, string authToken, int tryCount);
    
    [DllImport ("__Internal")]
    public static extern void showIdfaConsent(int type, int delay, int position, string titleText, string descriptionText, 
        string buttonText, string termsText, string policyText, string termsUrl, string policyUrl);
    
    [DllImport ("__Internal")]
    public static extern string getConsentStatus();
    
    [DllImport ("__Internal")]
    public static extern string getBuildNumber();
    
    [DllImport ("__Internal")]
    public static extern string getLocale();
    
    [DllImport ("__Internal")]
    public static extern void showAlertDialog(string title, string message);
    
    [DllImport ("__Internal")]
    public static extern void showForceUpdate(string title, string message);
    
    [DllImport("__Internal")]
    public static extern void showPopUpView(string subviewType, string text, string buttonTitle, string privacyPolicyText, 
                                            string privacyPolicyUrl, string termsOfServiceText, 
                                            string termsOfServiceUrl, 
                                            string dataRequestText = "", string dataRequestUrl = "");
    
    [DllImport("__Internal")]
    public static extern void showCcpaPopUpView(string action, string title, string content, 
                       string privacyPolicyText, string privacyPolicyUrl, 
                       string declineActionButtonText, string agreeActionButtonText,
                       string backToGameActionButtonText);

    [DllImport("__Internal")]
    public static extern void showSettingsView(string subviewType, string actions);
    
    [DllImport("__Internal")]
    public static extern void showBlockedPopUpView(string title, string content, string warningContent, string buttonTitle);

    [DllImport("__Internal")]
    public static extern void showNetworkOfflinePopUpView(string content, string buttonTitle);
    
    [DllImport("__Internal")]
    public static extern int gameMemoryUsage();
    
    [DllImport("__Internal")]
    public static extern int gameMemoryUsagePercent();
    
    [DllImport("__Internal")]
    public static extern long getFirstInstallTime();
#endif
}
