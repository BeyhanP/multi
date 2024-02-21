using System;
using UnityEngine;

namespace RollicGames.Advertisements
{

    public class RollicAdsAndroid
    {
#if UNITY_ANDROID
        private static AndroidJavaObject currentActivity =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        private static AndroidJavaObject rollicAdsAndroidController;

        public static void Init()
        {
            try
            {
                AndroidJavaClass rollicAdsCoreClass = new AndroidJavaClass("com.rollicads.RollicAdsController");
                rollicAdsAndroidController = rollicAdsCoreClass.CallStatic<AndroidJavaObject>("create", currentActivity);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public static float ConvertDpToPixel(float dp)
        {
            var pixel = 0.0f;
            if (rollicAdsAndroidController != null)
            {
                pixel = rollicAdsAndroidController.Call<float>("convertDpToPixel", dp);
            }

            return pixel;
        }
#endif
    }
}