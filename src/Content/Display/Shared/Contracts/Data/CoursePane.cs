namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CoursePane : Observable
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

    public Int32? IdSource
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CourseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RouteCourseId
    {
        get;
        set => SetProperty(ref field, value);
    }
}