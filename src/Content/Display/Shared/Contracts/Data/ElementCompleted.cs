namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ElementCompleted : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? DeviceTimestamp
    {
        get;
        set => SetProperty(ref field, value);
    }
}