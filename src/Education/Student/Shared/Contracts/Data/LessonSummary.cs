namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class LessonSummary : Observable, IUnique
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

    public Guid? ImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GuideText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideAudioId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequireSequentialCompletion
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? GeneratesAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile Image
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? RequiresAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ObjectiveCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ImageUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ObjectiveLite> Objectives
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}