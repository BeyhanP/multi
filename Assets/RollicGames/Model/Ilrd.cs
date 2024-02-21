using System;

namespace RollicGames.Advertisements.Model
{
    [Serializable]
    public class Ilrd
    {
        private double? revenue;
        private string networkName;
        private string adUnitId;
        private string placement;
        private string networkPlacement;
        private string creativeId;
        private string adFormat;

        public Ilrd(MaxSdk.AdInfo adInfo, string adFormat)
        {
            this.revenue = adInfo.Revenue;
            this.networkName = adInfo.NetworkName;
            this.adUnitId = adInfo.AdUnitIdentifier;
            this.placement = adInfo.Placement;
            this.networkPlacement = adInfo.NetworkPlacement;
            this.creativeId = adInfo.CreativeIdentifier;
            this.adFormat = adFormat;
        }
    }
}