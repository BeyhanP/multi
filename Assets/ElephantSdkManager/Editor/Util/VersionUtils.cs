using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ElephantSdkManager.Model;
using UnityEditor;
using UnityEngine;

namespace ElephantSdkManager.Util
{
    public static class VersionUtils
    {
        public static int CompareVersions(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

            var versionA = VersionStringToInts(a);
            var versionB = VersionStringToInts(b);
            for (var i = 0; i < Mathf.Max(versionA.Length, versionB.Length); i++)
            {
                if (VersionPiece(versionA, i) < VersionPiece(versionB, i))
                    return -1;
                if (VersionPiece(versionA, i) > VersionPiece(versionB, i))
                    return 1;
            }

            return 0;
        }

        public static bool IsEqualVersion(string a, string b)
        {
            return a.Equals(b);
        }


        private static int VersionPiece(IList<int> versionInts, int pieceIndex)
        {
            return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
        }


        private static int[] VersionStringToInts(string version)
        {
            int piece;
            if (version.Contains("_internal"))
            {
                version = version.Replace("_internal", string.Empty);
            }
            return version.Split('.')
                .Select(v => int.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out piece) ? piece : 0)
                .ToArray();
        }

        private static string CheckMediationPackageName(string packageName)
        {
            if (packageName.ToLower().Contains("gamekit-is"))
            {
                return Application.dataPath + "/RollicGames/RollicIronSourceIDs.cs";
            }

            if (packageName.ToLower().Contains("gamekit-max") || packageName.ToLower().Contains("gamekit-for"))
            {
                return Application.dataPath + "/RollicGames/RollicApplovinIDs.cs";
            }

            return null;
        }
        
        private static string GetElephantThirdParyIdsPath(string packageName)
        {
            if (packageName.ToLower().Contains("gamekit"))
            {
                return Application.dataPath + "/Elephant/Core/ElephantThirdPartyIds.cs";
            }

            return null;
        }
        
        public static void SetupElephantThirdPartyIDs(GameKitManifest gameKitManifest, string packageName)
        {
            if (gameKitManifest is null || gameKitManifest.data is null || gameKitManifest.data.appKey is null) return;
            
            var elephantPath = GetElephantThirdParyIdsPath(packageName);

            if (elephantPath is null) return;

            string[] lines = File.ReadAllLines(elephantPath);
            File.Delete(elephantPath);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Your IDs is being set...\n");

            using (StreamWriter sw = File.AppendText(elephantPath))
            {
                foreach (string line in lines)
                {
                    string newLine = "";
                    if (line.Contains("[TEMP_GAMEKIT_FacebookClientToken]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_FacebookClientToken]", gameKitManifest.data.facebookClientToken);
                        stringBuilder.Append("Facebook client token: " + gameKitManifest.data.facebookClientToken + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_FacebookAppId]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_FacebookAppId]", gameKitManifest.data.facebookAppId);
                        stringBuilder.Append("Facebook app ID: " + gameKitManifest.data.facebookAppId + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GameId]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GameId]",
                            gameKitManifest.data.gameId);
                        stringBuilder.Append("Elephant Game ID: " + gameKitManifest.data.gameId + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GameSecret]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GameSecret]",
                            gameKitManifest.data.gameSecret);
                        stringBuilder.Append("Elephant Game Secret: " + gameKitManifest.data.gameSecret + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_BundleName]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_BundleName]",
                            gameKitManifest.data.bundle);
                        
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android ,gameKitManifest.data.bundle);
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS ,gameKitManifest.data.bundle);
                        stringBuilder.Append("Game Bundle Name is set to: " + gameKitManifest.data.bundle + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_AdjustAppKey]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_AdjustAppKey]",
                            gameKitManifest.data.adjustAppKey);
                        stringBuilder.Append("Adjust App Key: " + gameKitManifest.data.adjustAppKey + "\n");
                    }
                    else
                    {
                        newLine = line;
                    }

                    sw.WriteLine(newLine);
                }
            }
            
            SetupAdjustTokens(gameKitManifest);
            
            Debug.Log(stringBuilder);
            
        }

        public static void SetupAdjustTokens(GameKitManifest gameKitManifest)
        {
            var adjustTokenClassPath = Application.dataPath + "/Elephant/Core/AdjustTokens.cs";

            if (!File.Exists(adjustTokenClassPath)) return;
            
            var lines = File.ReadAllLines(adjustTokenClassPath);
            File.Delete(adjustTokenClassPath);

            using (var sw = File.AppendText(adjustTokenClassPath))
            {
                foreach (var line in lines)
                {
                    var key = gameKitManifest.data.adjustEvents.Find(aEvent => line.Contains(aEvent.name));
                    var newLine = "";
                    if (key != null)
                    {
                        if (line.Contains("[TEMP_GAMEKIT_lvl100]"))
                        {
                            var key2 = gameKitManifest.data.adjustEvents.Find(aEvent => aEvent.name.Equals("lvl100"));
                            if (key2 != null)
                            {
                                newLine = line.Replace("[TEMP_GAMEKIT_" + key2.name + "]", key2.token);    
                            }
                        }
                        else
                        {
                            newLine = line.Replace("[TEMP_GAMEKIT_" + key.name + "]", key.token);
                        
                        }
                    }
                    else
                    {
                        newLine = line;
                    
                    }
                
                    sw.WriteLine(newLine);
                }
            }
        }

        public static void SetupGameKitIDs(GameKitManifest gameKitManifest, string packageName)
        {
            if (gameKitManifest is null || gameKitManifest.data is null || gameKitManifest.data.appKey is null) return;
            
            string rollicAdsPath = CheckMediationPackageName(packageName);

            if (rollicAdsPath is null) return;

            string[] lines = File.ReadAllLines(rollicAdsPath);
            File.Delete(rollicAdsPath);
            var stringBuilder = new StringBuilder();

            using (StreamWriter sw = File.AppendText(rollicAdsPath))
            {
                foreach (string line in lines)
                {
                    string newLine = "";
                    if (line.Contains("[TEMP_GAMEKIT_AppKey]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_AppKey]", gameKitManifest.data.appKey);
                        stringBuilder.Append("Applovin MAX App Key: " + gameKitManifest.data.appKey + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_AppKey_ios]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_AppKey_ios]", gameKitManifest.data.appKeyIos);
                        stringBuilder.Append("IS appkey iOS: " + gameKitManifest.data.appKeyIos + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_AppKey_android]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_AppKey_android]", gameKitManifest.data.appKeyAndroid);
                        stringBuilder.Append("IS appkey Android: " + gameKitManifest.data.appKeyAndroid + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_BannerAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_BannerAdUnitIos]", gameKitManifest.data.bannerAdUnitIos);
                        stringBuilder.Append("iOS Banner ad unit ID: " + gameKitManifest.data.bannerAdUnitIos + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_InterstitialAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_InterstitialAdUnitIos]",
                            gameKitManifest.data.interstitialAdUnitIos);
                        stringBuilder.Append("iOS Interstitial ad unit ID: " + gameKitManifest.data.interstitialAdUnitIos + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_RewardedAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_RewardedAdUnitIos]",
                            gameKitManifest.data.rewardedAdUnitIos);
                        stringBuilder.Append("iOS Rewarded Ad ad unit ID: " + gameKitManifest.data.rewardedAdUnitIos + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_BannerAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_BannerAdUnitAndroid]",
                            gameKitManifest.data.bannerAdUnitAndroid);
                        stringBuilder.Append("Android Banner ad unit ID: " + gameKitManifest.data.bannerAdUnitAndroid + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_InterstitialAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_InterstitialAdUnitAndroid]",
                            gameKitManifest.data.interstitialAdUnitAndroid);
                        stringBuilder.Append("Android Interstitial ad unit ID: " + gameKitManifest.data.interstitialAdUnitAndroid + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_RewardedAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_RewardedAdUnitAndroid]",
                            gameKitManifest.data.rewardedAdUnitAndroid);
                        stringBuilder.Append("Android Rewarded Ad ad unit ID: " + gameKitManifest.data.rewardedAdUnitAndroid + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GoogleAppIdIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GoogleAppIdIos]",
                            gameKitManifest.data.googleAppIdIos);
                        stringBuilder.Append("iOS Google app ID: " + gameKitManifest.data.googleAppIdIos + "\n");
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GoogleAppIdAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GoogleAppIdAndroid]",
                            gameKitManifest.data.googleAppIdAndroid);
                        stringBuilder.Append("Android Google app ID: " + gameKitManifest.data.googleAppIdAndroid + "\n");
                    }
                    else
                    {
                        newLine = line;
                    }

                    sw.WriteLine(newLine);
                }
            }
            
            Debug.Log(stringBuilder);
        }

        #region IronSource Utils

        public static string GetVersionFromXML(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string version = "";
            try
            {
                xmlDoc.LoadXml(File.ReadAllText("Assets/IronSource/Editor/" + fileName + ".xml"));
            }
            catch (Exception)
            {
                return version;
            }

            var unityVersion = xmlDoc.SelectSingleNode("dependencies/unityversion");
            if (unityVersion != null)
            {
                return (unityVersion.InnerText);
            }

            return version;
        }

        #endregion

        #region Max Utils

        public static Versions GetCurrentVersions(string dependencyPath)
        {
            XDocument dependency;
            try
            {
                dependency = XDocument.Load(dependencyPath);
            }
#pragma warning disable 0168
            catch (IOException exception)
#pragma warning restore 0168
            {
                // Couldn't find the dependencies file. The plugin is not installed.
                return new Versions();
            }

            // <dependencies>
            //  <androidPackages>
            //      <androidPackage spec="com.applovin.mediation:network_name-adapter:1.2.3.4" />
            //  </androidPackages>
            //  <iosPods>
            //      <iosPod name="AppLovinMediationNetworkNameAdapter" version="2.3.4.5" />
            //  </iosPods>
            // </dependencies>
            string androidVersion = null;
            string iosVersion = null;
            var dependenciesElement = dependency.Element("dependencies");
            if (dependenciesElement != null)
            {
                var androidPackages = dependenciesElement.Element("androidPackages");
                if (androidPackages != null)
                {
                    var adapterPackage = androidPackages.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("androidPackage")
                        && element.FirstAttribute.Name.LocalName.Equals("spec")
                        && element.FirstAttribute.Value.StartsWith("com.applovin"));
                    if (adapterPackage != null)
                    {
                        androidVersion = adapterPackage.FirstAttribute.Value.Split(':').Last();
                        // Hack alert: Some Android versions might have square brackets to force a specific version. Remove them if they are detected.
                        if (androidVersion.StartsWith("["))
                        {
                            androidVersion = androidVersion.Trim('[', ']');
                        }
                    }
                }

                var iosPods = dependenciesElement.Element("iosPods");
                if (iosPods != null)
                {
                    var adapterPod = iosPods.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("iosPod")
                        && element.FirstAttribute.Name.LocalName.Equals("name")
                        && element.FirstAttribute.Value.StartsWith("AppLovin"));
                    if (adapterPod != null)
                    {
                        iosVersion = adapterPod.Attributes()
                            .First(attribute => attribute.Name.LocalName.Equals("version")).Value;
                    }
                }
            }

            var currentVersions = new Versions();
            if (androidVersion != null && iosVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}_ios_{1}", androidVersion, iosVersion);
                currentVersions.Android = androidVersion;
                currentVersions.Ios = iosVersion;
            }
            else if (androidVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}", androidVersion);
                currentVersions.Android = androidVersion;
            }
            else if (iosVersion != null)
            {
                currentVersions.Unity = string.Format("ios_{0}", iosVersion);
                currentVersions.Ios = iosVersion;
            }

            return currentVersions;
        }

        public class Versions
        {
            public string Unity;
            public string Android;
            public string Ios;

            public override bool Equals(object value)
            {
                var versions = value as Versions;

                return versions != null
                       && Unity.Equals(versions.Unity)
                       && (Android == null || Android.Equals(versions.Android))
                       && (Ios == null || Ios.Equals(versions.Ios));
            }

            public bool HasEqualSdkVersions(Versions versions)
            {
                return versions != null
                       && AdapterSdkVersion(Android).Equals(AdapterSdkVersion(versions.Android))
                       && AdapterSdkVersion(Ios).Equals(AdapterSdkVersion(versions.Ios));
            }

            public override int GetHashCode()
            {
                return new { Unity, Android, Ios }.GetHashCode();
            }

            private static string AdapterSdkVersion(string adapterVersion)
            {
                var index = adapterVersion.LastIndexOf(".");
                return index > 0 ? adapterVersion.Substring(0, index) : adapterVersion;
            }
        }

        #endregion
    }
}