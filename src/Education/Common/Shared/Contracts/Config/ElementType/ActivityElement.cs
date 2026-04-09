namespace Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

public class ActivityElement : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Activity? Activity
    {
        get;
        set => SetProperty(ref field, value);
    }
}