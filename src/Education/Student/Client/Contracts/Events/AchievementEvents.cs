namespace Crudspa.Education.Student.Client.Contracts.Events;

public class ShowAchievement
{
    public StudentAchievement? StudentAchievement { get; set; }
}

public class MadeProgress : Observable
{
    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ImageUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }
}