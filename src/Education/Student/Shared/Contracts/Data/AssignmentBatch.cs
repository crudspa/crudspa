namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class AssignmentBatch : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GameId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Published
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GameKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GameTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OldBookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? OldBookKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? OldBookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NewBookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? NewBookKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? NewBookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ActivityAssignment> ActivityAssignments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}