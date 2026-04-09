namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class PageConfig : Observable
{
    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }
}