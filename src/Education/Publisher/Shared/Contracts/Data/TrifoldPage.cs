namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class TrifoldPage : Observable
{
    public Guid? TrifoldId
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