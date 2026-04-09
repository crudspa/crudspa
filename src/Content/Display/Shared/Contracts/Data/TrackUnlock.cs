namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class TrackUnlock : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }
}