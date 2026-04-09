namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class LessonUnlock : Observable
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

    public String? UnitTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile Image
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }
}