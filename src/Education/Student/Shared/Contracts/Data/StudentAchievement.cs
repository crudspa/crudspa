namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class StudentAchievement : Observable, IUnique
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

    public Guid? RelatedEntityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Earned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Achievement Achievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public StudentUnlocks Unlocks
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean IsNew
    {
        get;
        set => SetProperty(ref field, value);
    }
}