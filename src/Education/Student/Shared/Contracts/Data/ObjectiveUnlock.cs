namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ObjectiveUnlock : Observable
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

    public String? LessonTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? UnitTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile TrophyImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile UnitImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? LessonId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }
}