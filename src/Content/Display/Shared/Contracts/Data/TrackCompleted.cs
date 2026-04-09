namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class TrackCompleted : Observable, IUnique
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

    public Guid? TrackId
    {
        get;
        set => SetProperty(ref field, value);
    }
}