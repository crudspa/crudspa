namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class PageForPane : Observable
{
    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IList<Page> Pages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}