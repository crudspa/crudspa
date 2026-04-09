namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class AchievementContent : Observable
{
    public String? ContentType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContentTitle
    {
        get;
        set => SetProperty(ref field, value);
    }
}