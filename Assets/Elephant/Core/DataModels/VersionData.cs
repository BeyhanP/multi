using System;
using System.Collections.Generic;

namespace ElephantSDK
{
    [Serializable]
    public class VersionData
    {
        public string appVersion = "";
        public string sdkVersion = "";
        public string osVersion = "";
        public string mediationVersion = "";
        public string unityVersion = "";
        public string mediationName = "";
        public string gameKitVersion = "";

        public VersionData(string appVersion, string sdkVersion, string osVersion, string unityVersion, string gameKitVersion)
        {
            this.appVersion = appVersion;
            this.sdkVersion = sdkVersion;
            this.osVersion = osVersion;
            this.unityVersion = unityVersion;
            this.gameKitVersion = gameKitVersion;
        }
    }
    
}