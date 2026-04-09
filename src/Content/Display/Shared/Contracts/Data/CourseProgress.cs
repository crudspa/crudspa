namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CourseProgress : Observable
{
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

    public Int32? TimesCompleted
    {
        get;
        set => SetProperty(ref field, value);
    }
}