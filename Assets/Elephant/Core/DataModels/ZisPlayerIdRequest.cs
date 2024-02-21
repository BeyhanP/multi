using System;

namespace ElephantSDK
{
    [Serializable]
    public class ZisPlayerIdRequest : BaseData
    {
        public string locale;
        
        private ZisPlayerIdRequest()
        {
            
        }

        public static ZisPlayerIdRequest CreateZisPlayerIdRequest()
        {
            var a = new ZisPlayerIdRequest();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            return a;
        }
    }
}