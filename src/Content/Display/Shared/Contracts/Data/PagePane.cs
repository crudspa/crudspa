namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class PagePane : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }
}