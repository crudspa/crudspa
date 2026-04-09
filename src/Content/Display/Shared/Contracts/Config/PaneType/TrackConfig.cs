namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class TrackConfig : Observable
{
    public enum IdSources { FromUrl, SpecificTrack }

    public IdSources IdSource
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