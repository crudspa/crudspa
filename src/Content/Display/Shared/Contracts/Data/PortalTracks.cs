namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class PortalTracks : Observable
{
    public ObservableCollection<Track> Tracks
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}