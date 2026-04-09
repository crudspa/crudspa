namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ObjectivePage : Observable
{
    public Guid? ObjectiveId
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