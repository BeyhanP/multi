using AppLovinMax.Scripts.IntegrationManager.Editor;
using RollicGames.Advertisements;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace RollicGames.Editor
{
    public class Preprocess: IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
        
            AppLovinSettings.Instance.AdMobIosAppId = RollicApplovinIDs.GoogleIosId;
            AppLovinSettings.Instance.AdMobAndroidAppId = RollicApplovinIDs.GoogleAndroidId;
            AppLovinSettings.Instance.QualityServiceEnabled = false;
            EditorPrefs.SetBool(AppLovinAutoUpdater.KeyAutoUpdateEnabled, false);
            EditorUtility.SetDirty(AppLovinSettings.Instance);
        }
    }
}