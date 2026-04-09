namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class PortalStyles : Observable
{
    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Revision
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StyleCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Style> Styles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Font> Fonts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}