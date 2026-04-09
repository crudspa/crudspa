namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ReadPartCompleted : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ReadPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? DeviceTimestamp
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadTextEntry> TextEntries
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}