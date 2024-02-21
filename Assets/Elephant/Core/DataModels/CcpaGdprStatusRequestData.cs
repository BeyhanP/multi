using System;

namespace ElephantSDK
{
    [Serializable]
    public class CcpaGdprStatusRequestData: BaseData
    {
        public bool accepted;

        public static CcpaGdprStatusRequestData CreateCcpaGdprStatusRequestData(bool accepted)
        {
            var request = new CcpaGdprStatusRequestData();
            
            request.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            request.accepted = accepted;

            return request;
        }
    }
}