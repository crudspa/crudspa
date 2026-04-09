namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CourseCompleted : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CourseId
    {
        get;
        set => SetProperty(ref field, value);
    }
}