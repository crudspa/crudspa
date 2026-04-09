namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class StudentNotebook : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NotebookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
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