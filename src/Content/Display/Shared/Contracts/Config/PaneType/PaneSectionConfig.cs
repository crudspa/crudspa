namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class PaneSectionConfig : Observable
{
    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }
}