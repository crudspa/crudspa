namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ObjectiveLite : Observable, IUnique
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

    public Guid? LessonId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public ImageFile TrophyImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObjectiveProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new() { TimesCompleted = 0 };

    public Boolean Locked
    {
        get;
        set => SetProperty(ref field, value);
    }
}