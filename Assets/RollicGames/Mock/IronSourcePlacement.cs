public class IronSourcePlacement
{
    private string rewardName;
    private int rewardAmount;
    private string placementName;

    public IronSourcePlacement (string placementName = "", string rewardName = "", int rewardAmount = -1)
    {
        this.placementName = "";
        this.rewardName = "";
        this.rewardAmount = -1;
    }

    public string getRewardName ()
    {
        return "";
    }

    public int getRewardAmount ()
    {
        return -1;
    }

    public string getPlacementName ()
    {
        return "";
    }

    public override string ToString ()
    {
        return "";
    }
}