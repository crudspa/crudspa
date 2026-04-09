namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ObjectiveCompleted : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ObjectiveId
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