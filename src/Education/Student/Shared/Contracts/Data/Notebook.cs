namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Notebook : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NotebookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Notepage> Notepages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}