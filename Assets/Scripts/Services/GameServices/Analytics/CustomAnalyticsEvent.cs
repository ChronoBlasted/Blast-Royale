public class ChangeAreaEvent : Unity.Services.Analytics.Event
{
    public ChangeAreaEvent() : base("changeArea")
    {
    }

    public int ChangeAreaAmount { set { SetParameter("changeAreaAmount", value); } }
}