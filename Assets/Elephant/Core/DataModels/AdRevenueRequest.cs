using System;

namespace ElephantSDK
{
    [Serializable]
    public class AdRevenueRequest : BaseData
    {
        public string mopubRevenueData;
        public string mediationRevenueData;
        public string ironsourceRevenueData;
        
        private AdRevenueRequest()
        {
        }
        
        public static AdRevenueRequest CreateAdRevenueRequest(string mopubRevenueData)
        {
            var a = new AdRevenueRequest();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            a.mopubRevenueData = mopubRevenueData;
            a.mediationRevenueData = "";
            a.ironsourceRevenueData = "";
            return a;
        }
        
        public static AdRevenueRequest CreateMediationRevenueRequest(string mediationRevenueData)
        {
            var a = new AdRevenueRequest();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            a.mediationRevenueData = mediationRevenueData;
            a.mopubRevenueData = "";
            a.ironsourceRevenueData = "";
            return a;
        }
        
        public static AdRevenueRequest CreateIronSourceAdRevenueRequest(string ironsourceRevenueData)
        {
            var a = new AdRevenueRequest();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            a.mopubRevenueData = "";
            a.mediationRevenueData = "";
            a.ironsourceRevenueData = ironsourceRevenueData;
            return a;
        }
    }
}