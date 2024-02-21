namespace ElephantSDK
{
    public class ComplianceRequestData: BaseData
    {
        public ComplianceRequestData()
        {
            this.user_id = ElephantCore.Instance.userId;
        }
    }
}