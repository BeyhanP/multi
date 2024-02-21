using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ElephantSDK
{
    // TODO implement external package version checks here
    public class VersionCheckUtils
    {
        private const string MediationIronSource = "IronSource";
        
        private static VersionCheckUtils _instance;
        public string AdSdkVersion = "";
        public string MediationVersion = "";
        public readonly string UnityVersion;
        public string Mediation = "";

        public static VersionCheckUtils GetInstance()
        {
            if (_instance != null) return _instance;
            
            _instance = new VersionCheckUtils();

            return _instance;
        }

        private VersionCheckUtils()
        {
            PrepareVersions();
            UnityVersion = GetUnityVersion();
        }
        
        private void PrepareVersions()
        {
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    var typeFullName = type.FullName;
                    if (!string.IsNullOrEmpty(typeFullName) &&
                        typeFullName.Equals("RollicGames.Advertisements.Version"))
                    {
                        var fieldInfo = type.GetField("SDK_VERSION",
                            BindingFlags.NonPublic | BindingFlags.Static);

                        if (fieldInfo != null)
                        {
                            AdSdkVersion = fieldInfo.GetValue(null).ToString();    
                        }
                        
                    }

                    if (string.IsNullOrEmpty(typeFullName) || !typeFullName.Equals("IronSource")) continue;
                    var method = type.GetMethod("pluginVersion");

                    if (method == null) continue;
                    var result = method.Invoke(type, new object[] { });
                    MediationVersion = result.ToString();
                    MediationVersion = MediationVersion.Split('-')[0];
                    Mediation = MediationIronSource;
                }
                

             
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private static string GetUnityVersion() => Application.unityVersion;

        public int CompareVersions(string a, string b)
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
        
        private int VersionPiece(IList<int> versionInts, int pieceIndex)
        {
            return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
        }


        private int[] VersionStringToInts(string version)
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
    }
}