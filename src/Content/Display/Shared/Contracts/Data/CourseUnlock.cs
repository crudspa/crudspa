namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CourseUnlock : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TrackId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TrackTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TrackDescription
    {
        get;
        set => SetProperty(ref field, value);
    }
}