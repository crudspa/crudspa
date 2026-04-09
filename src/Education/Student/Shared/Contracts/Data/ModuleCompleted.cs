namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ModuleCompleted : Observable, IUnique
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

    public Guid? ModuleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
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