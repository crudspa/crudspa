namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class CourseConfig : Observable
{
    public enum IdSources { FromUrl, SpecificCourse }

    public IdSources IdSource
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