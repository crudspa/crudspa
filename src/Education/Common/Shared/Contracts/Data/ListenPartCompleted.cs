namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenPartCompleted : Observable, IUnique
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

    public Guid? ListenPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? DeviceTimestamp
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenTextEntry> TextEntries
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}